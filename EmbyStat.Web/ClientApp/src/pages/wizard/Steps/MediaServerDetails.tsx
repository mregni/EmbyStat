import React, {forwardRef, useContext, useImperativeHandle} from 'react';
import {Controller, useForm} from 'react-hook-form';
import {useTranslation} from 'react-i18next';

import OpenInNewIcon from '@mui/icons-material/OpenInNew';
import {
  Box, FormControl, FormHelperText, Grid, IconButton, MenuItem, Select, Stack, Typography,
} from '@mui/material';

import {StepProps, ValidationHandleWithSave} from '../';
import embyLogo from '../../../shared/assets/images/emby.png';
import jellyfinLogo from '../../../shared/assets/images/jellyfin.png';
import {EsTextInput} from '../../../shared/components/esTextInput';
import {WizardContext} from '../../../shared/context/wizard/WizardState';
import {useServerType} from '../../../shared/hooks';
import {MediaServer} from '../../../shared/models/mediaServer';

export const MediaServerDetails =
forwardRef<ValidationHandleWithSave, StepProps>(function MediaServerDetails(props, ref) {
  const {wizard, setMediaServerNetworkInfo} = useContext(WizardContext);
  const {getMediaServerTypeStringFromNumber} = useServerType();
  const {t} = useTranslation();

  useImperativeHandle(ref, () => ({
    async validate(): Promise<boolean> {
      await trigger();
      return Promise.resolve(isValid);
    },
    saveChanges(): void {
      if (isValid) {
        const {address, apiKey, type} = getValues();
        const mediaServer: MediaServer = {
          address: address,
          apiKey: apiKey,
          id: '',
          name: '',
          type: type,
        };
        setMediaServerNetworkInfo(mediaServer);
      }
    },
  }));

  const {register, trigger, getValues, formState: {errors, isValid}, control} = useForm({
    mode: 'onBlur',
    defaultValues: {
      address: wizard.address,
      apiKey: wizard.apiKey,
      type: wizard.serverType,
    },
  });

  const serverTypeList = [
    {id: 0, value: 0, label: 'Emby', logo: embyLogo},
    {id: 1, value: 1, label: 'Jellyfin', logo: jellyfinLogo},
  ];

  const openMediaServer = async () => {
    try {
      const {address, type} = getValues();
      const htmlSuffix = type === 1 ? '.html' : '';
      window.open(
        `${address}/web/index.html#!/apikeys${htmlSuffix}`,
        '_blank',
      );
    } catch {
      await trigger(['address']);
    }
  };

  const addressRegister = register('address', {required: true});
  const apiKeyRegister = register('apiKey', {required: true});

  return (
    <Stack spacing={3}>
      <Typography variant="h4" color="primary">
        {t('WIZARD.SERVERCONFIGURATION')}
      </Typography>
      <FormControl style={{width: '100%'}}>
        <Controller
          name="type"
          control={control}
          defaultValue={getValues('type')}
          render={({field}) => (
            <Select
              id="type"
              label={t('WIZARD.SERVERTYPELABEL')}
              className="max-width"
              variant="outlined"
              sx={{minWidth: 250}}
              {...field}
            >
              {serverTypeList.map((x) => (
                <MenuItem key={x.id} value={x.value}>
                  <Grid container spacing={1} alignItems="center">
                    <Grid item>
                      <img src={x.logo} height={20} alt="mediaserver logo" />
                    </Grid>
                    <Grid item>
                      {x.label}
                    </Grid>
                  </Grid>
                </MenuItem>
              ))}
            </Select>
          )}
        />
        <FormHelperText>{t('WIZARD.SERVERTYPEGHELPER')}</FormHelperText>
      </FormControl>
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
          helperText={
            t('SETTINGS.MEDIASERVER.APIKEYHINT',
              {type: getMediaServerTypeStringFromNumber(getValues('type'))},
            )
          }
        />
      </Box>
    </Stack>
  );
});
