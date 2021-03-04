import React from 'react'
import Grid from '@material-ui/core/Grid';

import ShowLibraryCard from './ShowLibraryCard';

const ShowSettings = () => {
  return (
    <Grid container spacing={2}>
      <Grid item xs={12} >
        <ShowLibraryCard delay={200} />
      </Grid>
    </Grid>
  )
}

export default ShowSettings
