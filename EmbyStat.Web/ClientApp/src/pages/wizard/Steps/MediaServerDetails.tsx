import React, { forwardRef, useContext, useEffect, useImperativeHandle, useState } from 'react'
import { Controller, useForm } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import Grid from '@material-ui/core/Grid';
import Select from '@material-ui/core/Select';
import MenuItem from '@material-ui/core/MenuItem';
import FormHelperText from '@material-ui/core/FormHelperText';
import FormControl from '@material-ui/core/FormControl';
import FormControlLabel from "@material-ui/core/FormControlLabel";
import Tooltip from '@material-ui/core/Tooltip';
import Switch from '@material-ui/core/Switch';
import Fade from '@material-ui/core/Fade';
import Typography from '@material-ui/core/Typography';
import InputLabel from '@material-ui/core/InputLabel';

import { WizardContext } from '../Context/WizardState';
import { StepProps, ValidationHandleWithSave } from '.'
import embyLogo from '../../../shared/assets/images/emby.png';
import jellyfinLogo from '../../../shared/assets/images/jellyfin.png';
import { EsTextInput } from '../../../shared/components/esTextInput';
import { EsButton } from '../../../shared/components/buttons';
import { MediaServer } from '../../../shared/models/mediaServer';


export const MediaServerDetails = forwardRef<ValidationHandleWithSave, StepProps>((props, ref) => {
  const { wizard, setMediaServerNetworkInfo } = useContext(WizardContext);
  const { t } = useTranslation();

  useImperativeHandle(ref, () => ({
    async validate(): Promise<boolean> {
      await trigger();
      console.log(errors);
      console.log(isValid);
      return Promise.resolve(isValid);
    },
    saveChanges(): void {
      if (isValid) {
        const { protocol, address, port, baseUrl, baseUrlNeeded, apiKey, type } = getValues();
        const mediaServer: MediaServer = {
          address: address,
          apiKey: apiKey,
          baseUrl: baseUrl,
          baseUrlNeeded: baseUrlNeeded,
          id: '',
          name: '',
          port: port,
          protocol: protocol,
          type: type
        };
        setMediaServerNetworkInfo(mediaServer);
      }
    }
  }));

  const { register, trigger, control, getValues, setValue, watch, formState: { errors, isValid } } = useForm({
    mode: "onBlur",
    defaultValues: {
      protocol: wizard.serverProtocol,
      address: wizard.serverAddress,
      port: wizard.serverPort,
      baseUrl: wizard.serverBaseurl,
      baseUrlNeeded: wizard.serverBaseUrlNeeded,
      apiKey: wizard.apiKey,
      type: wizard.serverType
    },
  });

  const protocolList = [
    { id: 0, value: 0, label: "https://" },
    { id: 1, value: 1, label: "http://" },
  ];

  const serverTypeList = [
    { id: 0, value: 0, label: "Emby", logo: embyLogo },
    { id: 1, value: 1, label: "Jellyfin", logo: jellyfinLogo },
  ];

  const openMediaServer = async () => {
    try {
      const { type, protocol, address, port, baseUrl, baseUrlNeeded } = getValues();
      const htmlPage = type === 0 ? "apikeys" : "apikeys.html";
      const protocolTxt = protocol === 0 ? "https://" : "http://";
      const baseUrlTxt = baseUrlNeeded ? baseUrl : '';
      window.open(
        `${protocolTxt}${address}:${port}${baseUrlTxt}/web/index.html#!/${htmlPage}`,
        "_blank"
      );
    }
    catch {
      await trigger(['address', 'port', 'baseUrl']);
    }

  };

  const { baseUrlNeeded } = watch();
  const addressRegister = register('address', { required: true });
  const portRegister = register('port', { required: true });
  const baseUrlRegister = register('baseUrl', { validate: (value) => { return !baseUrlNeeded || value.length !== 0; }, pattern: baseUrlNeeded ? /^\// : /\*/ });
  const apiKeyRegister = register('apiKey', { required: true });

  useEffect(() => {
    let didCancel = false;

    const triggerValidation = async () => {
      if (!didCancel) {
        if (!baseUrlNeeded) {
          setValue('baseUrl', '');
        }
        await trigger("baseUrl");
      }
    }

    triggerValidation();
    return () => {
      didCancel = true;
    };
  }, [baseUrlNeeded, setValue, trigger])

  return (
    <Grid
      container
      direction="column"
      spacing={2}
    >
      <Grid item>
        <Typography variant="h4" color="primary">
          {t('WIZARD.SERVERCONFIGURATION')}
        </Typography>
      </Grid>
      <Grid
        item
        container
        xs={12}
        direction="row"
      >
        <FormControl style={{ width: '100%' }}>
          <InputLabel htmlFor="type">{t('WIZARD.SERVERTYPELABEL')}</InputLabel>
          <Controller
            name="type"
            control={control}
            defaultValue={getValues('type')}
            render={({ field }) => (
              <Select
                id="type"
                className="max-width"
                variant="standard"
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
      </Grid>
      <Grid
        item
        container
        direction="row"
        spacing={2}
      >
        <Grid item xs={12} md={2}>
          <FormControl style={{ width: '100%' }}>
            <InputLabel htmlFor="protocol">{t('COMMON.PROTOCOL')}</InputLabel>
            <Controller
              name="protocol"
              control={control}
              defaultValue={getValues('protocol')}
              render={({ field }) => (
                <Select
                  id="protocol"
                  className="max-width"
                  variant="standard"
                  {...field}
                >
                  {protocolList.map((x) => (
                    <MenuItem key={x.id} value={x.value}>
                      {x.label}
                    </MenuItem>
                  ))}
                </Select>
              )}
            />
          </FormControl>
        </Grid>
        <Grid item xs={12} md={8}>
          <EsTextInput
            inputRef={addressRegister}
            defaultValue={getValues('address')}
            error={errors.address}
            label={t("SETTINGS.MEDIASERVER.ADDRESS")}
            errorText={{ required: t("SETTINGS.MEDIASERVER.NOADDRESS") }}
          />
        </Grid>
        <Grid item xs={12} md={2}>
          <EsTextInput
            inputRef={portRegister}
            defaultValue={getValues('port')}
            error={errors.port}
            label={t("SETTINGS.MEDIASERVER.PORT")}
            errorText={{ required: t("SETTINGS.MEDIASERVER.NOPORT") }}
          />
        </Grid>
      </Grid>
      <Grid
        item
        container
        direction="column"
      >
        <Grid item>
          <Controller
            name="baseUrlNeeded"
            control={control}
            defaultValue={getValues('baseUrlNeeded')}
            render={({ field }) => (
              <Tooltip
                title={t("SETTINGS.MEDIASERVER.BASEURLNEEDEDTOOLTIP")!}
                TransitionComponent={Fade}
                TransitionProps={{ timeout: 600 }}
              >
                <FormControlLabel
                  control={
                    <Switch
                      {...field}
                      color="primary"
                    />
                  }
                  label={t("SETTINGS.MEDIASERVER.BASEURLNEEDED")}
                />
              </Tooltip>
            )}
          />
        </Grid>
        <Grid item>
          <EsTextInput
            inputRef={baseUrlRegister}
            defaultValue={getValues('baseUrl')}
            error={errors.baseUrl}
            label={t("SETTINGS.MEDIASERVER.BASEURL")}
            errorText={{
              validate: t("SETTINGS.MEDIASERVER.NOBASEURL"),
              pattern: t("SETTINGS.MEDIASERVER.BASEURLHINT")
            }}
            helperText={t("SETTINGS.MEDIASERVER.BASEURLHINT")}
            readonly={!baseUrlNeeded}
          />
        </Grid>
      </Grid>
      <Grid
        item
        container
        direction="row"
        spacing={2}
        justify="center"
        alignItems="center"
      >
        <Grid item xs={12} md={3}>
          <EsButton
            onClick={openMediaServer}
          >
            {t("WIZARD.OPENSERVERAPIPAGE", {
              type: getValues('type') === 0 ? "Emby" : "Jellyfin",
            })}
          </EsButton>
        </Grid>
        <Grid item xs={12} md={9}>
          <EsTextInput
            inputRef={apiKeyRegister}
            defaultValue={getValues('apiKey')}
            error={errors.apiKey}
            label={t("SETTINGS.MEDIASERVER.APIKEY")}
            errorText={{ required: t("SETTINGS.MEDIASERVER.NOAPIKEY") }}
            helperText={t("SETTINGS.MEDIASERVER.APIKEYHINT", {
              type: getValues('type') === 0 ? "Emby" : "Jellyfin",
            })}
          />
        </Grid>
      </Grid>
    </Grid>
  )
})
