import React, { useState, useEffect } from 'react';
import { Grid, TextField, Button, FormControlLabel, Switch, Tooltip, Fade, makeStyles } from '@material-ui/core';
import { useDispatch } from 'react-redux';
import { useTranslation } from 'react-i18next';

import { Wizard } from '../../../../shared/models/wizard';
import { setMediaServerType, setMediaServerProtocol, setBaseUrlIsNeeded } from '../../../../store/WizardSlice';
import EmbyStatSelect from '../../../../shared/components/inputs/select/EmbyStatSelect';

const useStyles = makeStyles((theme) => ({
  'input-field__padding': {
    marginTop: 16,
    [theme.breakpoints.up('md')]: {
      marginTop: 0,
    }
  },
  'input-field__container': {
    height: 53,
    [theme.breakpoints.up('md')]: {
      paddingLeft: 8,
    }
  },
  'base-url__container': {
    marginTop: 16,
    minHeight: 110,
  },
}));

interface Props {
  className?: string,
  errors: any
  register: Function,
  wizard: Wizard
}

const ServerForm = (props: Props) => {
  const { className, register, errors, wizard } = props;
  const classes = useStyles();
  const [protocol, setProtocol] = useState(wizard.serverProtocol);
  const [address, setAddress] = useState(wizard.serverAddress);
  const [port, setPort] = useState(wizard.serverPort);
  const [apiKey, setApiKey] = useState(wizard.apiKey);
  const [type, setType] = useState<number>(wizard.serverType);
  const [baseUrl, setBaseUrl] = useState(wizard.serverBaseurl);
  const [baseUrlNeeded, setBaseUrlNeeded] = useState(wizard.serverBaseUrlNeeded);
  const { t } = useTranslation();
  const dispatch = useDispatch();

  useEffect(() => {
    setProtocol(wizard.serverProtocol);
    setAddress(wizard.serverAddress);
    setPort(wizard.serverPort);
    setApiKey(wizard.apiKey);
    setType(wizard.serverType);
    setBaseUrl(wizard.serverBaseurl);
  }, [wizard]);

  const protocolChanged = (event) => {
    setProtocol(event.target.value);
    dispatch(setMediaServerProtocol(event.target.value));
  }

  const portChanged = (event) => {
    setPort(event.target.value);
  }

  const typeChanged = (event) => {
    setType(event.target.value);
    dispatch(setMediaServerType(event.target.value))
  }

  const openMediaServer = () => {
    const htmlPage = type === 0 ? 'apikeys' : 'apikeys.html';
    const protocolTxt = protocol === 0 ? 'https://' : 'http://';
    window.open(`${protocolTxt}${address}:${port}${baseUrl}/web/index.html#!/${htmlPage}`, '_blank');
  }

  const baseUrlNeededChanged = (event) => {
    event.preventDefault();
    setBaseUrlNeeded(event.target.checked);
    setBaseUrl('');
    dispatch(setBaseUrlIsNeeded(event.target.checked));
  }

  const protocolList = [
    { id: 0, value: 0, label: 'https://', },
    { id: 1, value: 1, label: 'http://', },
  ]

  const serverTypeList = [
    { id: 0, value: 0, label: 'Emby', },
    { id: 1, value: 1, label: 'Jellyfin', },
  ]

  return (
    <Grid container direction="column" {...(className !== undefined && { className })}>
      <Grid item container xs={12} direction="row" justify="flex-end" className="m-t-16">
        <EmbyStatSelect
          className={classes["input-field__padding"]}
          variant="standard"
          value={type}
          onChange={typeChanged}
          menuItems={serverTypeList} />
      </Grid>
      <Grid item container direction="row" className="m-t-16">
        <Grid item xs={12} md={2}>
          <EmbyStatSelect
            className={classes["input-field__padding"]}
            variant="standard"
            value={protocol}
            onChange={protocolChanged}
            menuItems={protocolList} />
        </Grid>
        <Grid item xs={12} md={8} className={classes["input-field__container"]}>
          <TextField
            inputRef={register({ required: t('SETTINGS.MEDIASERVER.NOADDRESS') })}
            error={errors.address ? true : false}
            helperText={errors.address ? errors.address.message : ''}
            className={classes["input-field__padding"]}
            placeholder={t('SETTINGS.MEDIASERVER.ADDRESS')}
            value={address}
            name="address"
            variant="standard"
            onChange={(event) => setAddress(event.target.value)} />
        </Grid>
        <Grid item xs={12} md={2} className={classes["input-field__container"]}>
          <TextField
            inputRef={register({ required: t('SETTINGS.MEDIASERVER.NOPORT') })}
            error={errors.port ? true : false}
            helperText={errors.port ? errors.port.message : ''}
            className={classes["input-field__padding"]}
            value={port}
            name="port"
            type="number"
            inputProps={{ min: 0, max: 65535, step: 1 }}
            placeholder={t('SETTINGS.MEDIASERVER.PORT')}
            variant="standard"
            onChange={portChanged} />
        </Grid>
      </Grid>
      <Grid item container direction="column" className={classes["base-url__container"]}>
        <Grid item>
          <Tooltip title={t('SETTINGS.MEDIASERVER.BASEURLNEEDEDTOOLTIP')!} TransitionComponent={Fade} TransitionProps={{ timeout: 600 }}>
            <FormControlLabel
              control={
                <Switch
                  checked={baseUrlNeeded}
                  color="secondary"
                  onChange={baseUrlNeededChanged}
                />
              }
              label={t('SETTINGS.MEDIASERVER.BASEURLNEEDED')}
            />
          </Tooltip>
        </Grid>
        {baseUrlNeeded ? <Grid item xs={12}>
          <TextField
            inputRef={register({ required: baseUrlNeeded, pattern: /^\// })}
            error={errors.baseUrl ? true : false}
            helperText={errors.baseUrl ? t('SETTINGS.MEDIASERVER.NOBASEURL')
              : t('SETTINGS.MEDIASERVER.BASEURLHINT')}
            className={classes["input-field__padding"]}
            value={baseUrl}
            placeholder={t('SETTINGS.MEDIASERVER.BASEURL')}
            name="baseUrl"
            variant="standard"
            onChange={(event) => setBaseUrl(event.target.value)} />
        </Grid> : null}
      </Grid>
      <Grid item container xs={12} direction="row" justify="flex-end" className="m-t-16">
        <Button
          color="primary"
          disabled={address.length === 0 || !port || (baseUrlNeeded && !/^\//.test(baseUrl))}
          variant="contained"
          onClick={openMediaServer}>
          {t('WIZARD.OPENSERVERAPIPAGE', { type: type === 0 ? "Emby" : "Jellyfin" })}
        </Button>
      </Grid>
      <Grid item xs={12} className="m-t-16">
        <TextField
          inputRef={register({ required: t('SETTINGS.MEDIASERVER.NOAPIKEY') })}
          error={errors.apiKey ? true : false}
          helperText={errors.apiKey ? errors.apiKey.message
            : t('SETTINGS.MEDIASERVER.APIKEYHINT', { type: type === 0 ? "Emby" : "Jellyfin" })}
          className={classes["input-field__padding"]}
          value={apiKey}
          placeholder={t('SETTINGS.MEDIASERVER.APIKEY')}
          name="apiKey"
          variant="standard"
          onChange={(event) => setApiKey(event.target.value)} />
      </Grid>
    </Grid>
  )
}

ServerForm.defaultProps = {
  className: '',
}

export default ServerForm
