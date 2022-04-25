/* eslint-disable no-unused-vars */
import React, {forwardRef, useContext, useEffect} from 'react';
import {useTranslation} from 'react-i18next';

import {Grid, Stack, Typography} from '@mui/material';

import {EsLoading} from '../../../shared/components/esLoading';
import {WizardContext} from '../../../shared/context/wizard/WizardState';
import {MediaServer, MediaServerUdpBroadcast} from '../../../shared/models/mediaServer';
import {StepProps, ValidationHandleWithSave} from '../Interfaces';
import {NewServerCard, ServerCard} from './Helpers';

export const SearchMediaServer =
forwardRef<ValidationHandleWithSave, StepProps>((props: StepProps, ref) => {
  // eslint-disable-next-line react/prop-types
  const {handleNext} = props;
  const {t} = useTranslation();
  const {wizard, getMediaServers, mediaServersLoading, setMediaServerNetworkInfo} = useContext(WizardContext);

  useEffect(() => {
    getMediaServers();
  }, []);

  const onServerCardClick = async (server: MediaServerUdpBroadcast) => {
    const url = `http${server.protocol === 0 ? 's' : ''}://${server.address}:${server.port}${server.baseUrl ?? ''}`;
    const mediaServer: MediaServer = {
      address: url,
      apiKey: '',
      type: server.type,
      name: server.name,
      id: server.id,
    };

    setMediaServerNetworkInfo(mediaServer);
    if (handleNext !== undefined) {
      await handleNext();
    }
  };

  const addNewServer = async () => {
    const mediaServer: MediaServer = {
      address: '',
      apiKey: '',
      type: 0,
      name: '',
      id: '',
    };

    setMediaServerNetworkInfo(mediaServer);
    if (handleNext !== undefined) {
      await handleNext();
    }
  };

  return (
    <Stack>
      <Typography variant="h4" color="primary">
        {t('WIZARD.SERVERCONFIGURATION')}
      </Typography>
      <EsLoading
        loading={mediaServersLoading}
        label={t('WIZARD.SEARCHSERVERS')}
        width='100%'
        height='200px'
      >
        <Grid container={true} direction="column" sx={{minHeigt: 115}}>
          <Typography variant="body1" sx={{mb: 3}}>
            {wizard.foundServers.length > 0 ? t('WIZARD.FOUNDTEXT') : t('WIZARD.NOTFOUNDTEXT')}
          </Typography>
          <Grid item={true} container={true} direction="row" xs={12} spacing={2}>
            {wizard.foundServers.map((x: MediaServerUdpBroadcast) => (
              <ServerCard server={x} key={x.id} onClick={onServerCardClick} />
            ))}
            <NewServerCard onClick={addNewServer} />
          </Grid>
        </Grid>
      </EsLoading>
    </Stack>
  );
});
