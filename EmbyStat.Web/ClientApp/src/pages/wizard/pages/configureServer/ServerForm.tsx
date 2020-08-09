import classNames from 'classnames';
import React, { useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';

import Button from '@material-ui/core/Button';
import Fade from '@material-ui/core/Fade';
import FormControlLabel from '@material-ui/core/FormControlLabel';
import Grid from '@material-ui/core/Grid';
import MenuItem from '@material-ui/core/MenuItem';
import Select from '@material-ui/core/Select';
import { makeStyles } from '@material-ui/core/styles';
import Switch from '@material-ui/core/Switch';
import TextField from '@material-ui/core/TextField';
import Tooltip from '@material-ui/core/Tooltip';

import { Wizard } from '../../../../shared/models/wizard';

const useStyles = makeStyles((theme) => ({
  "input-field__padding": {
    marginTop: 16,
    [theme.breakpoints.up("md")]: {
      marginTop: 0,
    },
  },
  "input-field__container": {
    height: 53,
    [theme.breakpoints.up("md")]: {
      paddingLeft: 8,
    },
  },
  "base-url__container": {
    marginTop: 16,
    minHeight: 110,
  },
}));

interface Props {
  className?: string;
  errors: any;
  register: Function;
  wizard: Wizard;
  triggerValidation: Function;
  setValue: Function;
}

const ServerForm = (props: Props) => {
  const {
    className,
    register,
    errors,
    wizard,
    triggerValidation,
    setValue,
  } = props;
  const classes = useStyles();
  const [protocol, setProtocol] = useState(wizard.serverProtocol);
  const [address, setAddress] = useState(wizard.serverAddress);
  const [port, setPort] = useState(wizard.serverPort);
  const [apiKey, setApiKey] = useState(wizard.apiKey);
  const [type, setType] = useState<number>(wizard.serverType);
  const [baseUrl, setBaseUrl] = useState(wizard.serverBaseurl);
  const [baseUrlNeeded, setBaseUrlNeeded] = useState(
    wizard.serverBaseUrlNeeded
  );
  const { t } = useTranslation();

  useEffect(() => {
    setPort(wizard.serverPort);
    setAddress(wizard.serverAddress);
    setProtocol(wizard.serverProtocol);
    setApiKey(wizard.apiKey);
    setType(wizard.serverType);
    setBaseUrlNeeded(wizard.serverBaseUrlNeeded);
    if (wizard.serverBaseUrlNeeded) {
      if (wizard.serverBaseurl.startsWith("/")) {
        setBaseUrl(wizard.serverBaseurl);
      } else {
        setBaseUrl(`/${wizard.serverBaseurl}`);
      }
    }

    setValue("protocol", wizard.serverProtocol);
    setValue("type", wizard.serverType);
    setValue("baseUrlNeeded", wizard.serverBaseUrlNeeded);

    if (address !== wizard.serverAddress && address !== "") {
      triggerValidation(["address", "port"]);
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [wizard]);

  useEffect(() => {
    register({ name: "protocol" }, { required: true });
    register({ name: "type" }, { required: true });
    register({ name: "baseUrlNeeded" });
  }, [register]);

  const protocolChanged = (event) => {
    setValue("protocol", event.target.value);
    setProtocol(event.target.value);
  };

  const typeChanged = (event) => {
    setValue("type", event.target.value);
    setType(event.target.value);
  };

  const baseUrlNeededChanged = (event) => {
    event.preventDefault();
    setBaseUrlNeeded(event.target.checked);
    setBaseUrl("");
    setValue("baseUrlNeeded", event.target.value);
  };

  const openMediaServer = () => {
    const htmlPage = type === 0 ? "apikeys" : "apikeys.html";
    const protocolTxt = protocol === 0 ? "https://" : "http://";
    window.open(
      `${protocolTxt}${address}:${port}${baseUrl}/web/index.html#!/${htmlPage}`,
      "_blank"
    );
  };

  const protocolList = [
    { id: 0, value: 0, label: "https://" },
    { id: 1, value: 1, label: "http://" },
  ];

  const serverTypeList = [
    { id: 0, value: 0, label: "Emby" },
    { id: 1, value: 1, label: "Jellyfin" },
  ];

  return (
    <Grid
      container
      direction="column"
      {...(className !== undefined && { className })}
    >
      <Grid
        item
        container
        xs={12}
        direction="row"
        justify="flex-end"
        className="m-t-16"
      >
        <Select
          className={classNames(classes["input-field__padding"], "max-width")}
          variant="standard"
          onChange={typeChanged}
          value={type}
          name="type"
        >
          {serverTypeList.map((x) => (
            <MenuItem key={x.id} value={x.value}>
              {x.label}
            </MenuItem>
          ))}
        </Select>
      </Grid>
      <Grid item container direction="row" className="m-t-16">
        <Grid item xs={12} md={2}>
          <Select
            className={classNames(classes["input-field__padding"], "max-width")}
            variant="standard"
            onChange={protocolChanged}
            value={protocol}
            name="protocol"
          >
            {protocolList.map((x) => (
              <MenuItem key={x.id} value={x.value}>
                {x.label}
              </MenuItem>
            ))}
          </Select>
        </Grid>
        <Grid item xs={12} md={8} className={classes["input-field__container"]}>
          <TextField
            inputRef={register({
              required: t("SETTINGS.MEDIASERVER.NOADDRESS"),
            })}
            error={!!errors.address}
            helperText={errors.address ? errors.address.message : ""}
            className={classes["input-field__padding"]}
            placeholder={t("SETTINGS.MEDIASERVER.ADDRESS")}
            value={address}
            name="address"
            variant="standard"
            onChange={(event) => setAddress(event.target.value)}
          />
        </Grid>
        <Grid item xs={12} md={2} className={classes["input-field__container"]}>
          <TextField
            inputRef={register({ required: t("SETTINGS.MEDIASERVER.NOPORT") })}
            error={!!errors.port}
            helperText={errors.port ? errors.port.message : ""}
            className={classes["input-field__padding"]}
            value={port}
            name="port"
            type="number"
            inputProps={{ min: 0, max: 65535, step: 1 }}
            placeholder={t("SETTINGS.MEDIASERVER.PORT")}
            variant="standard"
            onChange={(event) => setPort(event.target.value)}
          />
        </Grid>
      </Grid>
      <Grid
        item
        container
        direction="column"
        className={classes["base-url__container"]}
      >
        <Grid item>
          <Tooltip
            title={t("SETTINGS.MEDIASERVER.BASEURLNEEDEDTOOLTIP")!}
            TransitionComponent={Fade}
            TransitionProps={{ timeout: 600 }}
          >
            <FormControlLabel
              control={
                <Switch
                  checked={baseUrlNeeded}
                  color="primary"
                  onChange={baseUrlNeededChanged}
                />
              }
              label={t("SETTINGS.MEDIASERVER.BASEURLNEEDED")}
            />
          </Tooltip>
        </Grid>
        {baseUrlNeeded ? (
          <Grid item xs={12}>
            <TextField
              inputRef={register({ required: baseUrlNeeded, pattern: /^\// })}
              error={!!errors.baseUrl}
              helperText={
                errors.baseUrl
                  ? t("SETTINGS.MEDIASERVER.NOBASEURL")
                  : t("SETTINGS.MEDIASERVER.BASEURLHINT")
              }
              className={classes["input-field__padding"]}
              value={baseUrl}
              placeholder={t("SETTINGS.MEDIASERVER.BASEURL")}
              name="baseUrl"
              variant="standard"
              onChange={(event) => setBaseUrl(event.target.value)}
            />
          </Grid>
        ) : null}
      </Grid>
      <Grid
        item
        container
        xs={12}
        direction="row"
        justify="flex-end"
        className="m-t-16"
      >
        <Button
          color="primary"
          disabled={
            address.length === 0 ||
            !port ||
            (baseUrlNeeded && !/^\//.test(baseUrl))
          }
          variant="contained"
          onClick={openMediaServer}
        >
          {t("WIZARD.OPENSERVERAPIPAGE", {
            type: type === 0 ? "Emby" : "Jellyfin",
          })}
        </Button>
      </Grid>
      <Grid item xs={12} className="m-t-16">
        <TextField
          inputRef={register({ required: t("SETTINGS.MEDIASERVER.NOAPIKEY") })}
          error={!!errors.apiKey}
          helperText={
            errors.apiKey
              ? errors.apiKey.message
              : t("SETTINGS.MEDIASERVER.APIKEYHINT", {
                  type: type === 0 ? "Emby" : "Jellyfin",
                })
          }
          className={classes["input-field__padding"]}
          value={apiKey}
          placeholder={t("SETTINGS.MEDIASERVER.APIKEY")}
          name="apiKey"
          variant="standard"
          onChange={(event) => setApiKey(event.target.value)}
        />
      </Grid>
    </Grid>
  );
};

ServerForm.defaultProps = {
  className: "",
};

export default ServerForm;
