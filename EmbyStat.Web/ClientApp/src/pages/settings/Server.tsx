import React, {useContext} from 'react';

import {Box, Grid, Stack} from '@mui/material';

import {EsTitle} from '../../shared/components/esTitle';
import {SettingsContext} from '../../shared/context/settings';
import {
  EsLanguageCard, EsMediaServerCard, EsPasswordCard, EsRollbarCard, EsTmdbApiCard, EsUserCard,
} from './components';

export const Server = () => {
  const {settings} = useContext(SettingsContext);

  if (settings === null) {
    return <></>;
  }

  return (
    <Stack spacing={2}>
      <EsTitle content='SETTINGS.SERVER.USER' isFirst />
      <Box>
        <Grid container spacing={2}>
          <Grid item xs={12} sm={6} lg={4} xl={3}>
            <EsUserCard />
          </Grid>
          <Grid item xs={12} sm={6} lg={4} xl={3}>
            <EsLanguageCard />
          </Grid>
          <Grid item xs={12} sm={6} lg={4} xl={3}>
            <EsPasswordCard />
          </Grid>
        </Grid>
      </Box>
      <EsTitle content='SETTINGS.SERVER.CONNECTIONS' isFirst />
      <Box>
        <Grid container spacing={2}>
          <Grid item xs={12} sm={12} lg={8} xl={3}>
            <EsMediaServerCard />
          </Grid>
          <Grid item xs={12} sm={6} lg={4} xl={3}>
            <EsTmdbApiCard />
          </Grid>
          <Grid item xs={12} sm={6} lg={4} xl={3}>
            <EsRollbarCard />
          </Grid>
        </Grid>
      </Box>
    </Stack>
  );
};
