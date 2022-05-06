import React from 'react';

import Grid from '@mui/material/Grid';

import LogoGif from '../../assets/images/running-logo.gif';

export function EsPageLoader() {
  return (
    <Grid
      container
      justifyContent="center"
      alignItems="center"
      sx={{
        width: '100vw',
        height: '100vh',
      }}
    >
      <Grid item>
        <img src={LogoGif} alt="running logo" width="500" />
      </Grid>
    </Grid>
  );
}
