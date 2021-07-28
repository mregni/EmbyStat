import React from 'react'
import Grid from '@material-ui/core/Grid';

import { MovieLibraryCard, StatisticsCard } from '.';

export const MovieSettings = () => {
  return (
    <Grid container spacing={2}>
      <Grid item xs={12} >
        <MovieLibraryCard delay={200} />
      </Grid>
      <Grid item xs={12} md={6} lg={4}>
        <StatisticsCard delay={225} />
      </Grid>
    </Grid>
  )
}
