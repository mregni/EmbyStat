import React, { useState, useEffect } from 'react';
import Dialog from '@material-ui/core/Dialog';
import DialogTitle from '@material-ui/core/DialogTitle';
import DialogContent from '@material-ui/core/DialogContent';
import DialogContentText from '@material-ui/core/DialogContentText';
import DialogActions from '@material-ui/core/DialogActions';
import Button from '@material-ui/core/Button';
import TextField from '@material-ui/core/TextField';
import { makeStyles } from '@material-ui/core/styles';
import { fade } from '@material-ui/core/styles/colorManipulator';
import { useTranslation } from 'react-i18next';
import { useForm } from 'react-hook-form';
import red from '@material-ui/core/colors/red';
import { useServerType } from '../../../shared/hooks';
import { Job } from '../../../shared/models/jobs';

import { updateTrigger } from '../../../shared/services/JobService';
import SaveButton from '../../../shared/components/buttons/SaveButton';
import SnackbarUtils from '../../../shared/utils/SnackbarUtilsConfigurator';

const useStyles = makeStyles((theme) => ({
  button__red: {
    color: red.A700,
    '&:hover': {
      backgroundColor: fade(red['200'], 0.08),
    },
  },
  link: {
    '& a': {
      color:
        theme.palette.type === 'dark'
          ? theme.palette.secondary.light
          : theme.palette.secondary.dark,
    },
  },
}));

interface Props {
  openSettingsDialog: boolean;
  job: Job;
}

const JobSettingsDialog = (props: Props) => {
  const classes = useStyles();
  const { openSettingsDialog, job } = props;
  const [openSettings, setOpenSettings] = useState(false);
  const [cron, setCron] = useState(job.trigger);
  const [changed, setChanged] = useState(false);
  const [isSaving, setIsSaving] = useState(false);
  const serverType = useServerType();

  const { t } = useTranslation();

  useEffect(() => {
    setOpenSettings(openSettingsDialog);
  }, [openSettingsDialog]);

  // eslint-disable-next-line max-len
  const cronPattern = /^(([*])|((([*]|([0-9]|[1-5][0-9]))[/])*([0-9]|[1-5][0-9]))|(([0-9]|[1-5][1-9])([,]([0-9]|[1-5][0-9]))+)|(([0-9]|[1-5][0-9])-([0-9]|[1-5][0-9]))) (([*])|([0-9]|1[0-9]|2[0-3])|((([*]|([0-9]|1[0-9]|2[0-3]))[/])*([0-9]|1[0-9]|2[0-3]))|((([0-9]|1[0-9]|2[0-3])([,]([0-9]|1[0-9]|2[0-3]))+))|(([0-9]|1[0-9]|2[0-3])-([0-9]|1[0-9]|2[0-3]))) (([*])|([1-9]|[1-2][0-9]|3[0-1])|((([*]|([1-9]|[1-2][0-9]|3[0-1]))[/])*([1-9]|[1-2][0-9]|3[0-1]))|((([1-9]|[1-2][0-9]|3[0-1])([,]([1-9]|[1-2][0-9]|3[0-1]))+))|(([1-9]|[1-2][0-9]|3[0-1])-([1-9]|[1-2][0-9]|3[0-1]))) (([*])|([1-9]|1[0-2])|((([*]|([1-9]|1[0-2]))[/])*([1-9]|1[0-2]))|((([1-9]|1[0-2])([,]([1-9]|1[0-2]))+))|(([1-9]|1[0-2])-([1-9]|1[0-2]))) (([*])|([0-7])|([0-7]([,][0-7])+)|([0-7]-[0-7]))$/;
  const { register, handleSubmit, errors } = useForm({
    mode: 'onBlur',
    defaultValues: {
      cron: job.trigger,
    },
  });

  const handleChange = (event) => {
    setChanged(true);
    setCron(event.target.value);
  };

  const safeChanges = (data) => {
    setIsSaving(true);
    updateTrigger(job.id, data.cron)
      .then(() => {
        setOpenSettings(false);
        SnackbarUtils.success(t('JOB.CRONUPDATED'));
      })
      .catch((error) => {
        setIsSaving(false);
      });
  };

  const helperText = { __html: t('DIALOGS.EDIT_TRIGGER.CRON_HELPER') };
  const errorText = { __html: t('DIALOGS.EDIT_TRIGGER.FORMAT_ERROR') };

  return (
    <Dialog
      open={openSettings}
      keepMounted
      onClose={() => setOpenSettings(false)}
    >
      <form autoComplete="off" onSubmit={handleSubmit(safeChanges)}>
        <DialogTitle>
          {t(`JOB.INFO.${job.title}`, { type: serverType })}
        </DialogTitle>
        <DialogContent>
          <DialogContentText>
            {t(`JOB.INFO.${job.description}`)}
          </DialogContentText>
          <TextField
            inputRef={register({ pattern: cronPattern })}
            label={t('FORMLABELS.CRON')}
            size="small"
            name="cron"
            disabled={isSaving}
            error={!!errors.cron}
            helperText={
              errors.cron ? (
                <span
                  className={classes.link}
                  dangerouslySetInnerHTML={errorText}
                />
              ) : (
                  <span
                    className={classes.link}
                    dangerouslySetInnerHTML={helperText}
                  />
                )
            }
            value={cron}
            onChange={handleChange}
          />
        </DialogContent>
        <DialogActions>
          <Button
            onClick={() => setOpenSettings(false)}
            color="secondary"
            disabled={isSaving}
            classes={{
              textSecondary: classes.button__red,
            }}
          >
            {changed ? t('COMMON.DISCARD') : t('COMMON.CANCEL')}
          </Button>
          <SaveButton hasError={!!errors.cron} isSaving={isSaving} />
        </DialogActions>
      </form>
    </Dialog>
  );
};

export default JobSettingsDialog;
