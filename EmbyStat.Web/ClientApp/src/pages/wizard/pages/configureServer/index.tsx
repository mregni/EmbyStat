import React, { useState, useEffect } from 'react'
import { Grid, Typography, makeStyles } from '@material-ui/core'
import { Trans, useTranslation } from 'react-i18next'
import classNames from 'classnames';

import { searchMediaServers } from '../../../../shared/services/MediaServerService';
import { MediaServerUdpBroadcast } from '../../../../shared/models/mediaServer';
import Loading from '../../../../shared/components/loading';
import ServerResult from './ServerResult';
import ServerForm from './ServerForm';

import { useSelector, useDispatch } from 'react-redux';
import { RootState } from '../../../../store/RootReducer';
import { setServerConfiguration, setFoundServers, setMovieLibraryStepLoaded, setShowLibraryStepLoaded, setBaseUrlIsNeeded } from '../../../../store/WizardSlice';

const useStyles = makeStyles((theme) => ({
  result__container: {
    minHeight: 115,
  },
}));

interface Props {
  errors: any
  register: Function,
  disableNext: Function,
  disableBack: Function,
  reset: Function,
}

const ConfigureServer = (props: Props) => {
  const classes = useStyles();
  const dispatch = useDispatch();
  const { t } = useTranslation();
  const { register, errors, disableNext, disableBack, reset } = props;
  const [servers, setServers] = useState<MediaServerUdpBroadcast[]>([]);
  const [isLoading, setIsLoading] = useState(true);

  const wizard = useSelector((state: RootState) => state.wizard);
  useEffect(() => {
    const searchServers = async () => {
      if (!wizard.searchedServers) {
        const servers = await searchMediaServers();
        setServers(servers);
        dispatch(setFoundServers(servers, true));
        setIsLoading(false);
      } else {
        setIsLoading(false);
        setServers(wizard.foundServers);
      }
    }

    searchServers();
  }, [isLoading, wizard.searchedServers, wizard.foundServers, dispatch]);

  useEffect(() => {
    console.log(wizard);
  }, [wizard])

  useEffect(() => {
    disableBack(false);
    disableNext(false);
  }, [disableNext, disableBack]);

  useEffect(() => {
    dispatch(setMovieLibraryStepLoaded(false));
    dispatch(setShowLibraryStepLoaded(false));
  }, [dispatch]);

  const changeSeletctedServer = async (server: MediaServerUdpBroadcast) => {
    console.log(server);
    reset();
    dispatch(setServerConfiguration(server.address, server.port, '', '', server.type, server.protocol));
    dispatch(setBaseUrlIsNeeded(false));
  }

  return (
    <Grid container direction="column">
      <Typography variant="h4" color="secondary">
        <Trans i18nKey="WIZARD.SERVERCONFIGURATION" />
      </Typography>
      <Loading
        className={classNames("m-t-32", classes.result__container)}
        loading={isLoading}
        label={t('WIZARD.SEARCHSERVERS')}
        Component={ServerResult}
        servers={servers}
        setSelectedServer={changeSeletctedServer}
      />
      <ServerForm
        register={register}
        errors={errors}
        wizard={wizard}
        className="m-t-32" />
    </Grid>
  )
}

export default ConfigureServer
