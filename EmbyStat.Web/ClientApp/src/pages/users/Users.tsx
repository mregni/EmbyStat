import React, {useContext, useEffect} from 'react';
import {useTranslation} from 'react-i18next';

import {Box, Grid, Stack} from '@mui/material';

import {EsLoading} from '../../shared/components/esLoading';
import {EsTitle} from '../../shared/components/esTitle';
import {
  MediaServerUserContext, MediaServerUserContextProvider,
} from '../../shared/context/mediaServerUser';
import {UserOverviewCard} from './components';

const UserContainer = () => {
  const {isLoaded, load, users} = useContext(MediaServerUserContext);
  const {t} = useTranslation();

  useEffect(() => {
    (async () => {
      if (!isLoaded) {
        await load();
      }
    })();
  }, [isLoaded]);

  return (
    <EsLoading label={t('USERS.LOADER')} loading={!isLoaded}>
      <Stack spacing={2}>
        <EsTitle content={t('USERS.NUMBERS')} isFirst />
        <EsTitle content={t('USERS.USERS')} />
        <Box>
          <Grid container spacing={2}>
            {
              users.map((user) =>
                <Grid item key={user.id} xs={12} md={4} lg={3}>
                  <UserOverviewCard user={user}/>
                </Grid>)
            }
          </Grid>
        </Box>
      </Stack>
    </EsLoading>
  );
};

export const Users = () => {
  return (
    <MediaServerUserContextProvider>
      <UserContainer/>
    </MediaServerUserContextProvider>
  );
};
