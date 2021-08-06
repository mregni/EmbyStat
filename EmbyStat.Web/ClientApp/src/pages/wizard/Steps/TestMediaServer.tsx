import React, { forwardRef, useContext, useEffect, useImperativeHandle, useRef, useState } from 'react'
import Grid from '@material-ui/core/Grid';
import Typography from '@material-ui/core/Typography';
import { useTranslation } from 'react-i18next';
import { makeStyles } from '@material-ui/core/styles';
import Zoom from '@material-ui/core/Zoom';
import Card from '@material-ui/core/Card';
import CardContent from '@material-ui/core/CardContent';
import MenuItem from '@material-ui/core/MenuItem';
import classNames from 'classnames';

import { StepProps, ValidationHandleWithSave } from '.'
import { EsLoading } from '../../../shared/components/loading';
import { WizardContext } from '../Context/WizardState';
import { MediaServerLogin } from '../../../shared/models/mediaServer';
import {
  testApiKey,
  getServerInfo,
  getAdministrators,
  getLibraries,
} from "../../../shared/services/MediaServerService";
import { Controller, useForm } from 'react-hook-form';
import MediaServerHeader from '../../../shared/components/mediaServerHeader';
import { EsTextInput } from '../../../shared/components/esTextInput';
import Select from '@material-ui/core/Select';
import FormControl from '@material-ui/core/FormControl';

const useStyles = makeStyles((theme) => ({
  root: {
    display: 'flex',
    padding: '8px',
    position: 'relative',
  },
  "input-field__padding": {
    marginTop: 16,
    [theme.breakpoints.up("md")]: {
      marginTop: 0,
    },
  },
  content: {
    flex: '1 0 auto',
  },
  cover: {
    width: 80,
    height: 80,
    padding: '10px',
  },
  server__details__header: {
    color:
      theme.palette.type === 'dark'
        ? theme.palette.grey[400]
        : theme.palette.grey[600],
    fontSize: '0.8rem',
  },
  server__details__name: {
    paddingLeft: '8px',
  },
  server__details__icon: {
    width: '20px',
    position: 'absolute',
    right: 5,
    top: 5,
    '&:hover': {
      cursor: 'pointer',
    },
  },
  italic: {
    fontStyle: 'italic',
  },
}));

const TestFailed = () => {
  const { wizard } = useContext(WizardContext);
  const { t } = useTranslation();
  const [type] = useState(wizard.serverType === 0 ? 'Emby' : 'Jellyfin');
  const [address, setAddress] = useState('');

  useEffect(() => {
    const protocolTxt = wizard.serverProtocol === 0 ? 'https://' : 'http://';
    const wizardAddress = wizard.serverAddress;
    const port = wizard.serverPort;
    const baseUrl = wizard?.serverBaseurl ?? '';
    setAddress(`${protocolTxt}${wizardAddress}:${port}${baseUrl}`);
  }, [wizard]);

  return (
    <Grid container direction="column">
      <Typography variant="body1" className="m-t-16">
        {t('WIZARD.APIKEYFAILED', { type, address, key: wizard.apiKey })}
      </Typography>
      <Typography className="m-t-32">
        {t('WIZARD.ADDRESSUSED', { address: address })}
      </Typography>
      <Typography>
        {t('WIZARD.APIKEYUSED', { api: wizard.apiKey })}
      </Typography>
    </Grid>
  )
}

const TestSuccessFul = forwardRef<ValidationHandleWithSave>((props, ref) => {
  const classes = useStyles();
  const { wizard, setMediaServerNetworkInfo, setAdministrator, fullServerUrl } = useContext(WizardContext);
  const { t } = useTranslation();

  useImperativeHandle(ref, () => ({
    async validate(): Promise<boolean> {
      const { address, selectedAddress } = getValues();

      if (selectedAddress !== 'other') {
        return Promise.resolve(true);
      }

      await trigger();
      var addressArray = (address).split('://');
      if (isValid) {
        if (!['http', 'https'].includes(addressArray[0])) {
          setError('address', { type: "protocol" })
          return Promise.resolve(false);
        }

        if (addressArray[1].split(':')[0].length === 0) {
          setError('address', { type: 'url' })
          return Promise.resolve(false);
        }

        if (addressArray[1].split(':')[1].split('/')[0] === undefined ||
          isNaN(parseInt(addressArray[1].split(':')[1].split('/')[0], 10))) {
          setError('address', { type: 'port' })
          return Promise.resolve(false);
        }
      }

      return Promise.resolve(true);
    },
    saveChanges(): void {
      const { address, selectedAddress, selectedAdmin } = getValues();
      var addressArray = selectedAddress === 'other' ? (address).split('://') : selectedAddress.split('://');

      const protocol = addressArray[0] === 'https' ? 0 : 1;
      const url = addressArray[1].split(':')[0];
      const port = parseInt(addressArray[1].split(':')[1].split('/')[0], 10);
      const base = addressArray[1].split(':')[1].split('/')[1];
      setMediaServerNetworkInfo({
        address: url,
        apiKey: wizard.apiKey,
        baseUrl: base,
        baseUrlNeeded: base !== undefined,
        id: wizard.serverId,
        name: wizard.serverName,
        port: port,
        protocol: protocol,
        type: wizard.serverType
      });

      const admin = wizard.administrators.find(x => x.id === selectedAdmin);
      if (admin !== undefined) {
        setAdministrator(admin.id);
      }
    }
  }));

  const { register, getValues, trigger, watch, control, setError,
    clearErrors, formState: { isValid, errors } } = useForm({
      mode: "onBlur",
      defaultValues: {
        address: '',
        selectedAddress: fullServerUrl,
        selectedAdmin: wizard.administrators[0].id
      }
    });

  const selectedAddress = watch('selectedAddress');
  const addressRegister = register('address', { validate: (value: string) => selectedAddress !== "other" || value.length > 0 });

  useEffect(() => {
    clearErrors('address')
  }, [selectedAddress, clearErrors])

  return (
    <Grid item container xs={12} direction="column">
      <Grid item>
        <MediaServerHeader
          serverType={wizard.serverType}
          serverInfo={wizard.mediaServerInfo}
          serverAddress={fullServerUrl} />
      </Grid>
      <Grid item>
        <Zoom in={true} style={{ transitionDelay: '300ms' }}>
          <Card
            elevation={7}
            square
            className={classNames(classes.root, 'm-t-32')}
          >
            <CardContent>
              <Grid container direction="column" spacing={2}>
                <Grid item>
                  {t('WIZARD.LANWANCHOISE', { type: wizard.serverType === 0 ? "Emby" : "Jellyfin" })}
                </Grid>
                <Grid item>
                  <FormControl style={{ width: '100%' }}>
                    <Controller
                      name="selectedAddress"
                      control={control}
                      render={({ field }) => (
                        <Select
                          className={classNames(classes["input-field__padding"], "max-width")}
                          variant="standard"
                          {...field}
                        >
                          {
                            fullServerUrl !== wizard.mediaServerInfo.wanAddress
                            && fullServerUrl !== wizard.mediaServerInfo.localAddress
                            && <MenuItem value={fullServerUrl}>
                              {t('COMMON.CURRENT')}&nbsp;<span className={classes.italic}>({fullServerUrl})</span>
                            </MenuItem>
                          }
                          {
                            wizard.mediaServerInfo.wanAddress !== null
                            && <MenuItem value={wizard.mediaServerInfo.wanAddress}>
                              WAN&nbsp;<span className={classes.italic}>({wizard.mediaServerInfo.wanAddress})</span>
                            </MenuItem>
                          }
                          <MenuItem value={wizard.mediaServerInfo.localAddress}>
                            LAN&nbsp;<span className={classes.italic}>({wizard.mediaServerInfo.localAddress})</span>
                          </MenuItem>
                          <MenuItem value="other">{t('COMMON.OTHER')}</MenuItem>
                        </Select>
                      )}
                    />
                  </FormControl>
                </Grid>
                <Grid item>
                  <EsTextInput
                    readonly={selectedAddress !== "other"}
                    inputRef={addressRegister}
                    defaultValue={getValues('address')}
                    error={errors.address}
                    errorText={{
                      validate: t('SETTINGS.MEDIASERVER.NOADDRESS'),
                      protocol: 'Protocol should be http or https',
                      url: 'URL be longer then 0 characters',
                      port: 'Port should be a number between 1 and 65535'
                    }}
                    label={t('SETTINGS.MEDIASERVER.ADDRESS')}
                    className={classes["input-field__padding"]}
                  />
                </Grid>
              </Grid>
            </CardContent>
          </Card>
        </Zoom>
      </Grid>
      <Grid item>
        {wizard.serverType === 1 ? (
          <Zoom in={true} style={{ transitionDelay: '300ms' }}>
            <Card
              elevation={7}
              square
              className={classNames(classes.root, 'm-t-32')}
            >
              <CardContent>
                {t('WIZARD.JELLYFIN.ADMINTEXT')}
                <Controller
                  name="selectedAdmin"
                  control={control}
                  render={({ field }) => (
                    <Select
                      className={classNames(classes["input-field__padding"], "max-width")}
                      variant="standard"
                      {...field}
                    >
                      {
                        wizard.administrators.map((admin) => (
                          <MenuItem value={admin.id} key={admin.id}>{t(admin.name)}</MenuItem>
                        ))
                      }
                    </Select>)}
                />
              </CardContent>
            </Card>
          </Zoom>
        ) : null}
      </Grid>
    </Grid>
  );
});

export const TestMediaServer = forwardRef<ValidationHandleWithSave, StepProps>((props, ref) => {
  const { t } = useTranslation();
  const { wizard, setMediaServerInfo,
    setAdministrators, setLibraries,
    setMovieLibraries, setShowLibraries } = useContext(WizardContext);

  const [loadingLabel, setLoadingLabel] = useState("WIZARD.STEPONE");
  const [isLoading, setIsLoading] = useState(true);
  const [currentStep, setCurrentStep] = useState(1);
  const [inError, setInError] = useState(false);

  const successRef = useRef<React.ElementRef<typeof TestSuccessFul>>(null);

  useImperativeHandle(ref, () => ({
    async validate(): Promise<boolean> {
      return await successRef.current?.validate() ?? Promise.resolve(false);
    },
    saveChanges(): void {
      successRef.current?.saveChanges();
    }
  }));

  useEffect(() => {
    const performSteps = async () => {
      const protocolTxt = wizard.serverProtocol === 0 ? "https://" : "http://";
      const address = wizard.serverAddress;
      const port = wizard.serverPort;
      const baseUrl = wizard?.serverBaseurl ?? "";
      const fullAddress = `${protocolTxt}${address}:${port}${baseUrl}`;

      if (currentStep === 1) {
        const result = await testApiKey({
          address: fullAddress,
          apiKey: wizard.apiKey,
          type: wizard.serverType,
        } as MediaServerLogin);

        if (result == null) {
          setIsLoading(false);
          return;
        }

        if (result) {
          setCurrentStep(2);
          setLoadingLabel("WIZARD.STEPTWO");
          const serverInfo = await getServerInfo(true);
          if (serverInfo == null) {
            setIsLoading(false);
            return;
          }

          setCurrentStep(3);
          setLoadingLabel("WIZARD.STEPTHREE");
          const admins = await getAdministrators();
          if (admins == null) {
            setIsLoading(false);
            return;
          }

          setCurrentStep(4);
          setLoadingLabel("WIZARD.STEPFOUR");
          const libs = await getLibraries();
          if (libs == null) {
            setIsLoading(false);
            return;
          }
          setLibraries(libs);
          setMovieLibraries(libs
            .filter((x) => x.type === 1)
            .map((x) => { return { id: x.id, name: x.name, lastSynced: null } }));
          setShowLibraries(libs
            .filter((x) => x.type === 2)
            .map((x) => { return { id: x.id, name: x.name, lastSynced: null } }));
          setMediaServerInfo(serverInfo);
          setAdministrators(admins);
          setIsLoading(false);
        } else {
          setInError(true);
          setIsLoading(false);
        }
      }
    };
    performSteps();
  }, [currentStep, wizard, isLoading, setMediaServerInfo, setAdministrators,
    setLibraries, setMovieLibraries, setShowLibraries]);

  return (
    <Grid
      container
      direction="column"
      spacing={2}
    >
      <Grid item>
        <Typography variant="h4" color="primary">
          {t('WIZARD.SERVERCONFIGURATIONTEST')}
        </Typography>
      </Grid>
      <Grid item>
        <EsLoading
          className="m-t-32"
          loading={isLoading}
          label={t(loadingLabel, { step: currentStep, total: 4 })}
        >
          {!inError ? <TestSuccessFul ref={successRef} /> : <TestFailed />}
        </EsLoading>
      </Grid>
    </Grid>
  )
})
