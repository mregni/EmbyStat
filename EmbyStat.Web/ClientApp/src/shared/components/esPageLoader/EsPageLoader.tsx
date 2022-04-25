import React from 'react';
import Grid from '@mui/material/Grid';

import LogoGif from '../../assets/images/running-logo.gif';

export function EsPageLoader() {
  return (
    <Grid
      container={true}
      justifyContent="center"
      alignItems="center"
      sx={{
        width: '100vw',
        height: '100vh',
      }}
    >
      <Grid item={true}>
        <img src={LogoGif} alt="running logo" width="500" />
      </Grid>
    </Grid>
  );
}
