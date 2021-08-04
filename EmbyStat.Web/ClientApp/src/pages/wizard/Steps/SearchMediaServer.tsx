import Grid from '@material-ui/core/Grid';
import { makeStyles } from '@material-ui/core/styles';
import Typography from '@material-ui/core/Typography';
import React, { forwardRef, useContext, useEffect } from 'react'
import { useTranslation } from 'react-i18next';

import { ValidationHandleWithSave, StepProps } from '.'
import { Loading } from '../../../shared/components/loading';
import { MediaServer, MediaServerUdpBroadcast } from '../../../shared/models/mediaServer';
import { WizardContext } from '../Context/WizardState';
import { NewServerCard, ServerCard } from './Helpers';

const useStyles = makeStyles(() => ({
  result__container: {
    minHeight: 115,
  },
}));

export const SearchMediaServer = forwardRef<ValidationHandleWithSave, StepProps>((props, ref) => {
  const { handleNext } = props;
  const { t } = useTranslation();
  const classes = useStyles();
  const { wizard, getMediaServers, mediaServersLoading, setMediaServerNetworkInfo } = useContext(WizardContext);

  useEffect(() => {
    getMediaServers();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  const onServerCardClick = async (server: MediaServerUdpBroadcast) => {
    const mediaServer: MediaServer = {
      address: server.address,
      baseUrl: server.baseUrl,
      port: server.port,
      protocol: server.protocol,
      apiKey: '',
      type: server.type,
      baseUrlNeeded: server.baseUrl !== null,
      name: server.name,
      id: server.id,
    };

    setMediaServerNetworkInfo(mediaServer);
    if (handleNext !== undefined) {
      await handleNext();
    }
  }

  const addNewServer = async () => {
    const mediaServer: MediaServer = {
      address: '',
      baseUrl: '',
      port: null,
      protocol: 0,
      apiKey: '',
      type: 0,
      baseUrlNeeded: false,
      name: '',
      id: ''
    };

    setMediaServerNetworkInfo(mediaServer);
    if (handleNext !== undefined) {
      await handleNext();
    }
  }

  return (
    <Grid container direction="column">
      <Typography variant="h4" color="primary">
        {t('WIZARD.SERVERCONFIGURATION')}
      </Typography>
      <Loading
        className={classes.result__container}
        loading={mediaServersLoading}
        label={t("WIZARD.SEARCHSERVERS")}
      >
        <Grid container direction="column" className={classes.result__container}>
          <Typography variant="body1" className="m-b-32">
            {t('WIZARD.FOUNDTEXT')}
          </Typography>

          <Grid item container direction="row" xs={12} spacing={2}>
            {wizard.foundServers.map((x: MediaServerUdpBroadcast) => (
              <ServerCard server={x} key={x.id} onClick={onServerCardClick} />
            ))}
            <NewServerCard onClick={addNewServer} />
          </Grid>
        </Grid>
      </Loading>
    </Grid>
  )
})
