import {t} from 'i18next';
import React, {useContext, useEffect, useState} from 'react';
import {useForm} from 'react-hook-form';

import {
  Box, Button, Dialog, DialogActions, DialogContent, DialogContentText, DialogTitle, Theme,
} from '@mui/material';

import {EsSaveButton} from '../../shared/components/buttons';
import {EsTextInput} from '../../shared/components/esTextInput';
import {JobsContext} from '../../shared/context/jobs';
import {useServerType} from '../../shared/hooks';
import {Job} from '../../shared/models/jobs';
import SnackbarUtils from '../../shared/utils/SnackbarUtilsConfigurator';

type Props = {
  openSettingsDialog: boolean;
  job: Job;
  onClose: () => void;
};

type Cron = {
  cron: string;
};

export function JobSettingsDialog(props: Props) {
  const {openSettingsDialog, job, onClose} = props;
  const [changed, setChanged] = useState(false);
  const [isSaving, setIsSaving] = useState(false);
  const [openSettings, setOpenSettings] = useState(false);
  const serverType = useServerType();
  const {updateJobTrigger} = useContext(JobsContext);

  useEffect(() => {
    setOpenSettings(openSettingsDialog);
  }, [openSettingsDialog]);

  const cronPattern = /^(([*])|((([*]|([0-9]|[1-5][0-9]))[/])*([0-9]|[1-5][0-9]))|(([0-9]|[1-5][1-9])([,]([0-9]|[1-5][0-9]))+)|(([0-9]|[1-5][0-9])-([0-9]|[1-5][0-9]))) (([*])|([0-9]|1[0-9]|2[0-3])|((([*]|([0-9]|1[0-9]|2[0-3]))[/])*([0-9]|1[0-9]|2[0-3]))|((([0-9]|1[0-9]|2[0-3])([,]([0-9]|1[0-9]|2[0-3]))+))|(([0-9]|1[0-9]|2[0-3])-([0-9]|1[0-9]|2[0-3]))) (([*])|([1-9]|[1-2][0-9]|3[0-1])|((([*]|([1-9]|[1-2][0-9]|3[0-1]))[/])*([1-9]|[1-2][0-9]|3[0-1]))|((([1-9]|[1-2][0-9]|3[0-1])([,]([1-9]|[1-2][0-9]|3[0-1]))+))|(([1-9]|[1-2][0-9]|3[0-1])-([1-9]|[1-2][0-9]|3[0-1]))) (([*])|([1-9]|1[0-2])|((([*]|([1-9]|1[0-2]))[/])*([1-9]|1[0-2]))|((([1-9]|1[0-2])([,]([1-9]|1[0-2]))+))|(([1-9]|1[0-2])-([1-9]|1[0-2]))) (([*])|([0-7])|([0-7]([,][0-7])+)|([0-7]-[0-7]))$/;
  const {register, handleSubmit, getValues, reset, formState: {errors}} = useForm<Cron>({
    mode: 'onBlur',
    defaultValues: {
      cron: job.trigger,
    },
  });

  const handleChange = () => {
    setChanged(true);
  };

  const safeChanges = async (data: Cron) => {
    setIsSaving(true);

    const result = await updateJobTrigger(job.id, data.cron);
    if (result) {
      SnackbarUtils.success(t('JOB.CRONUPDATED'));
      handleClose();
    } else {
      setIsSaving(false);
    }
  };

  const handleClose = () => {
    setIsSaving(false);
    reset();
    onClose();
  };

  const helperText = {__html: t('DIALOGS.EDIT_TRIGGER.CRON_HELPER')};
  const errorText = {__html: t('DIALOGS.EDIT_TRIGGER.FORMAT_ERROR')};
  const cronRegister = register('cron', {pattern: cronPattern});

  return (
    <>
      {
        openSettings ?
          <Dialog open={openSettings} onClose={() => handleClose()} >
            <form autoComplete="off" onSubmit={handleSubmit(safeChanges)}>
              <DialogTitle>
                {t(`JOB.INFO.${job.title}`, {type: serverType})}
              </DialogTitle>
              <DialogContent>
                <DialogContentText>
                  {t(`JOB.INFO.${job.description}`)}
                </DialogContentText>
                <EsTextInput
                  inputRef={cronRegister}
                  defaultValue={getValues('cron')}
                  helperText={<Box sx={{
                    '& a': {
                      color:
                  (theme: Theme) => theme.palette.mode === 'dark' ?
                    theme.palette.secondary.light : theme.palette.secondary.dark,
                    },
                  }} dangerouslySetInnerHTML={helperText} />
                  }
                  error={errors.cron}
                  errorText={{pattern: <Box
                    sx={{
                      '& a': {
                        color:
                    (theme: Theme) => theme.palette.mode === 'dark' ?
                      theme.palette.secondary.light : theme.palette.secondary.dark,
                      },
                    }} dangerouslySetInnerHTML={errorText} />,
                  }}
                  label="FORMLABELS.CRON"
                  onChange={handleChange}
                  readonly={isSaving}
                />
              </DialogContent>
              <DialogActions>
                <Button
                  onClick={() => handleClose()}
                  color="error"
                  disabled={isSaving}
                >
                  {changed ? t('COMMON.DISCARD') : t('COMMON.CANCEL')}
                </Button>
                <EsSaveButton
                  disabled={!!errors.cron}
                  isSaving={isSaving}
                />
              </DialogActions>
            </form>
          </Dialog> :
          null
      }
    </>
  );
}
