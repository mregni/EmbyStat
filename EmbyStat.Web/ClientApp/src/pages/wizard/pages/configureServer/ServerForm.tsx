import classNames from "classnames";
import React, { useEffect, useState } from "react";
import { useTranslation } from "react-i18next";

import FormControl from '@material-ui/core/FormControl';
import FormHelperText from "@material-ui/core/FormHelperText";
import Button from "@material-ui/core/Button";
import Fade from "@material-ui/core/Fade";
import FormControlLabel from "@material-ui/core/FormControlLabel";
import Grid from "@material-ui/core/Grid";
import MenuItem from "@material-ui/core/MenuItem";
import Select from "@material-ui/core/Select";
import { makeStyles } from "@material-ui/core/styles";
import Switch from "@material-ui/core/Switch";
import Tooltip from "@material-ui/core/Tooltip";

import { Wizard } from "../../../../shared/models/wizard";
import { EsTextInput } from "../../../../shared/components/esTextInput";
import { Controller } from "react-hook-form";

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
  getValues: Function;
  control: any
}

export const ServerForm = (props: Props) => {
  const {
    className = '',
    register,
    errors,
    wizard,
    triggerValidation,
    setValue,
    getValues,
    control
  } = props;
  const classes = useStyles();
  const { t } = useTranslation();
  const [address, setAddress] = useState(wizard.serverAddress);
  const [port, setPort] = useState(wizard.serverPort);
  const [baseUrl, setBaseUrl] = useState<string>(wizard.serverBaseurl);
  const [baseUrlNeeded, setBaseUrlNeeded] = useState<boolean>(
    wizard.serverBaseUrlNeeded
  );

  useEffect(() => {
    setBaseUrlNeeded(wizard.serverBaseUrlNeeded);

    if (wizard.serverBaseUrlNeeded) {
      if (wizard.serverBaseurl.startsWith("/")) {
        setValue("baseUrl", wizard.serverBaseurl);
      } else {
        setValue("baseUrl", `/${wizard.serverBaseurl}`);
      }
    } else {
      setValue("baseUrl", "");
    }

    //TODO => Wizard state werkt hier voor geen meter. Misschien toch zien om useForm in deze component te zetten al
    // => Nog meer refactoring :(
    setValue("type", wizard.serverType);
    setValue("protocol", wizard.serverProtocol);
    setValue("port", wizard.serverPort);
    setValue("address", wizard.serverAddress);
    setValue("baseUrlNeeded", wizard.serverBaseUrlNeeded);
    setValue("apiKey", wizard.apiKey);

    if (address !== wizard.serverAddress && address !== "") {
      triggerValidation(["address", "port"]);
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [wizard]);

  const baseUrlNeededChanged = (event) => {
    event.preventDefault();
    setBaseUrlNeeded(event.target.checked);
    setValue("baseUrlNeeded", event.target.checked);
    setBaseUrl("");
    setValue("baseUrl", "");
  };

  const baseUrlChanged = (value: string) => {
    setBaseUrl(value);
  };

  const openMediaServer = () => {
    const { type } = getValues('type');
    const htmlPage = type === 0 ? "apikeys" : "apikeys.html";
    const { protocol } = getValues('protocol');
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

  const typeRegister = register('type');
  const protocolRegister = register('protocol');
  const addressRegister = register('address', { required: true });
  const portRegister = register('port', { required: true });
  const baseUrlRegister = register('baseUrl', { required: baseUrlNeeded, pattern: /^\// });
  const apiKeyRegister = register('apiKey', { required: true });

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
        <FormControl style={{ width: '100%' }}>
          <Controller
            name="type"
            control={control}
            render={() => (
              <Select
                className={classNames(classes["input-field__padding"], "max-width")}
                variant="standard"
                inputRef={typeRegister}
              >
                {serverTypeList.map((x) => (
                  <MenuItem key={x.id} value={x.value}>
                    {x.label}
                  </MenuItem>
                ))}
              </Select>
            )}
          />
          <FormHelperText>{t('WIZARD.SERVERTYPEGHELPER')}</FormHelperText>
        </FormControl>
      </Grid>
      <Grid item container direction="row" className="m-t-16">
        <Grid item xs={12} md={2}>
          <FormControl fullWidth style={{ marginTop: 23 }}>
            <Controller
              name="protocol"
              control={control}
              render={() => (
                <Select
                  className={classNames(classes["input-field__padding"], "max-width")}
                  variant="standard"
                  inputRef={protocolRegister}
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
        <Grid item xs={12} md={8} className={classes["input-field__container"]}>
          <EsTextInput
            inputRef={addressRegister}
            defaultValue={getValues('address')}
            error={errors.address}
            className={classes["input-field__padding"]}
            label={t("SETTINGS.MEDIASERVER.ADDRESS")}
            errorText={{ required: t("SETTINGS.MEDIASERVER.NOADDRESS") }}
            onChange={(value: string) => setAddress(value)}
          />
        </Grid>
        <Grid item xs={12} md={2} className={classes["input-field__container"]}>
          <EsTextInput
            inputRef={portRegister}
            defaultValue={getValues('port')}
            error={errors.port}
            className={classes["input-field__padding"]}
            label={t("SETTINGS.MEDIASERVER.PORT")}
            errorText={{ required: t("SETTINGS.MEDIASERVER.NOPORT") }}
            onChange={(value: string) => setPort(0)}
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
            <EsTextInput
              inputRef={baseUrlRegister}
              defaultValue={getValues('baseUrl')}
              error={errors.baseUrl}
              className={classes["input-field__padding"]}
              label={t("SETTINGS.MEDIASERVER.BASEURL")}
              errorText={{ required: t("SETTINGS.MEDIASERVER.NOBASEURL") }}
              helperText={t("SETTINGS.MEDIASERVER.BASEURLHINT")}
              onChange={baseUrlChanged}
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
            type: getValues('type') === 0 ? "Emby" : "Jellyfin",
          })}
        </Button>
      </Grid>
      <Grid item xs={12} className="m-t-16">
        <EsTextInput
          inputRef={apiKeyRegister}
          defaultValue={getValues('apiKey')}
          error={errors.apiKey}
          className={classes["input-field__padding"]}
          label={t("SETTINGS.MEDIASERVER.APIKEY")}
          errorText={{ required: t("SETTINGS.MEDIASERVER.NOAPIKEY") }}
          helperText={t("SETTINGS.MEDIASERVER.APIKEYHINT", {
            type: getValues('type') === 0 ? "Emby" : "Jellyfin",
          })}
          onChange={(value: string) => setApiKey(value)}
        />
      </Grid>
    </Grid>
  );
};
