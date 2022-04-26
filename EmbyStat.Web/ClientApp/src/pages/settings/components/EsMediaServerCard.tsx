import React, {useContext, useEffect, useState} from 'react';
import {useForm} from 'react-hook-form';
import {useTranslation} from 'react-i18next';

import OpenInNewIcon from '@mui/icons-material/OpenInNew';
import {
  Backdrop, Box, Button, CircularProgress, Dialog, DialogActions, DialogContent, DialogTitle,
  IconButton, Stack, Typography,
} from '@mui/material';

import {EsSaveCard} from '../../../shared/components/cards';
import {EsTextInput} from '../../../shared/components/esTextInput';
import {SettingsContext} from '../../../shared/context/settings';
import {useServerType} from '../../../shared/hooks';
import {resetEmbyStat} from '../../../shared/services';

export interface ResetDialogProps {
  open: boolean;
  onCancel: () => void;
  onSave: (reset: boolean) => void;
}

function ResetDialog(props: ResetDialogProps) {
  const {onCancel, onSave, open} = props;
  const {t} = useTranslation();

  return (
    <Dialog onClose={onCancel} open={open}>
      <DialogTitle>{t('SETTINGS.MEDIASERVER.DIALOG.TITLE')}</DialogTitle>
      <DialogContent>
        <Typography variant="body1">
          {t('SETTINGS.MEDIASERVER.DIALOG.CONTENT')}
        </Typography>
      </DialogContent>
      <DialogActions>
        <Box display="flex" justifyContent="space-between" sx={{width: '100%'}}>
          <Box>
            <Button variant="outlined" onClick={onCancel}>{t('COMMON.CANCEL')}</Button>
          </Box>
          <Box>
            <Button
              variant="contained"
              color="error"
              onClick={() => onSave(true)}
            >
              {t('SETTINGS.MEDIASERVER.DIALOG.RESET')}
            </Button>
            <Button
              variant="contained"
              color="primary"
              onClick={() => onSave(false)}
              sx={{ml: 1}}
            >
              {t('COMMON.SAVE')}
            </Button>
          </Box>
        </Box>
      </DialogActions>
    </Dialog>
  );
}


type MediaServerForm = {
  address: string;
  apiKey: string;
}

export function EsMediaServerCard() {
  const {serverType} = useServerType();
  const {settings, save, resetFinished, resetLogLine} = useContext(SettingsContext);
  const [openBackdrop, setOpenBackdrop] = useState(false);
  const [openWarningDialog, setOpenWarningDialog] = useState(false);
  const {t} = useTranslation();

  const {register, handleSubmit, getValues, formState: {errors}} = useForm<MediaServerForm>({
    mode: 'all',
    defaultValues: {
      address: settings.mediaServer.address,
      apiKey: settings.mediaServer.apiKey,
    },
  });

  useEffect(() => {
    if (resetFinished) {
      setOpenBackdrop(false);
    }
  }, [resetFinished]);


  const processForm = (data: MediaServerForm) => {
    setOpenWarningDialog(true);
  };

  const saveForm = async (reset: boolean) => {
    const {address, apiKey} = getValues();
    settings.mediaServer.address = address;
    settings.mediaServer.apiKey = apiKey;
    setOpenWarningDialog(false);

    await save(settings);
    if (reset) {
      setOpenBackdrop(true);
      await resetEmbyStat();
    }
  };

  const addressRegister = register('address', {required: true});
  const apiKeyRegister = register('apiKey', {required: true});

  const openMediaServer = () => {
    const {address} = getValues();
    const htmlSuffix = serverType !== 'Emby' ? '.html' : '';
    window.open(`${address}/web/index.html#!/apikeys${htmlSuffix}`, '_blank');
  };

  return (
    <EsSaveCard
      title={t('SETTINGS.MEDIASERVER.TITLE', {type: serverType})}
      saveForm={processForm}
      handleSubmit={handleSubmit}
    >
      <Stack spacing={3}>
        <Box display="flex">
          <Box sx={{flex: 1}}>
            <EsTextInput
              inputRef={addressRegister}
              defaultValue={getValues('address')}
              error={errors.address}
              label={t('SETTINGS.MEDIASERVER.ADDRESS')}
              errorText={{required: t('SETTINGS.MEDIASERVER.NOADDRESS')}}
              helperText={t('SETTINGS.MEDIASERVER.ADDRESSHINT')}
            />
          </Box>
          <Box sx={{flex: 0}}>
            <IconButton color="primary" component="span" onClick={openMediaServer} sx={{ml: 0.5}}>
              <OpenInNewIcon/>
            </IconButton>
          </Box>
        </Box>
        <Box>
          <EsTextInput
            inputRef={apiKeyRegister}
            defaultValue={getValues('apiKey')}
            error={errors.apiKey}
            label={t('SETTINGS.MEDIASERVER.APIKEY')}
            errorText={{required: t('SETTINGS.MEDIASERVER.NOAPIKEY')}}
            helperText={t('SETTINGS.MEDIASERVER.APIKEYHINT', {type: serverType})}
          />
        </Box>
      </Stack>
      <ResetDialog
        open={openWarningDialog}
        onCancel={() => setOpenWarningDialog(false)}
        onSave={saveForm}
      />
      <Backdrop
        sx={{color: '#fff', zIndex: (theme) => theme.zIndex.drawer + 1, backgroundColor: '#000000d9'}}
        open={openBackdrop}
      >
        <Stack
          justifyContent="center"
          alignItems="center"
          spacing={2}
        >
          <CircularProgress color="inherit" />
          <Typography variant="body1" sx={{maxWidth: 450}} align="center">
            {t('SETTINGS.MEDIASERVER.RESETINFO')}
          </Typography>
          <Typography variant="body1" sx={{maxWidth: 450}} align="left">
            {resetLogLine}
          </Typography>
        </Stack>
      </Backdrop>
    </EsSaveCard>
  );
}
