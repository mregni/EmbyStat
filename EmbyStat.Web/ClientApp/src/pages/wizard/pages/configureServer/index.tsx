import React, { useState, useEffect } from 'react';
import Grid from '@material-ui/core/Grid';
import Typography from '@material-ui/core/Typography';
import { makeStyles } from '@material-ui/core/styles';
import { Trans, useTranslation } from 'react-i18next';
import classNames from 'classnames';
import { useSelector, useDispatch } from 'react-redux';

import { searchMediaServers } from '../../../../shared/services/MediaServerService';
import { MediaServerUdpBroadcast } from '../../../../shared/models/mediaServer';
import Loading from '../../../../shared/components/loading';
import ServerResult from './ServerResult';
import ServerForm from './ServerForm';
import { RootState } from '../../../../store/RootReducer';
import {
  setServerConfiguration,
  setFoundServers,
  setMovieLibraryStepLoaded,
  setShowLibraryStepLoaded,
} from '../../../../store/WizardSlice';

const useStyles = makeStyles((theme) => ({
  result__container: {
    minHeight: 115,
  },
}));

interface Props {
  errors: any;
  register: Function;
  disableNext: Function;
  disableBack: Function;
  triggerValidation: Function;
  setValue: Function;
}

const ConfigureServer = (props: Props) => {
  const classes = useStyles();
  const dispatch = useDispatch();
  const { t } = useTranslation();
  const { register, errors, disableNext, disableBack, triggerValidation, setValue } = props;
  const [servers, setServers] = useState<MediaServerUdpBroadcast[]>([]);
  const [isLoading, setIsLoading] = useState(true);

  const wizard = useSelector((state: RootState) => state.wizard);
  useEffect(() => {
    const searchServers = async () => {
      if (!wizard.searchedServers) {
        const newServers = await searchMediaServers();
        setServers(newServers);
        dispatch(setFoundServers(newServers, true));
        setIsLoading(false);
      } else {
        setIsLoading(false);
        setServers(wizard.foundServers);
      }
    };

    searchServers();
  }, [isLoading, wizard.searchedServers, wizard.foundServers, dispatch]);

  useEffect(() => {
    disableBack(false);
    disableNext(false);
  }, [disableNext, disableBack]);

  useEffect(() => {
    dispatch(setMovieLibraryStepLoaded(false));
    dispatch(setShowLibraryStepLoaded(false));
  }, [dispatch]);

  const changeSeletctedServer = async (server: MediaServerUdpBroadcast) => {
    dispatch(
      setServerConfiguration(
        server.address,
        server.port,
        server.baseUrl != null ? server.baseUrl : '',
        '',
        server.type,
        server.protocol,
        server.baseUrl != null ? true : false
      )
    );
  };

  return (
    <Grid container direction="column">
      <Typography variant="h4" color="primary">
        <Trans i18nKey="WIZARD.SERVERCONFIGURATION" />
      </Typography>
      <Loading
        className={classNames('m-t-32', classes.result__container)}
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
        triggerValidation={triggerValidation}
        className="m-t-32"
        setValue={setValue}
      />
    </Grid>
  );
};

export default ConfigureServer;
