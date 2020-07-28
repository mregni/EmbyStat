import React from 'react'
import Grid from '@material-ui/core/Grid';

import MovieLibraryCard from './MovieLibraryCard';
import StatisticsCard from './StatisticsCard';

const MovieSettings = () => {
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

export default MovieSettings
