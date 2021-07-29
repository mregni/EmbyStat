import React from 'react';
import Grid from '@material-ui/core/Grid';

import { LanguageCard } from './LanguageCard';
import { RollbarCard } from './RollbarCard';
import { TmdbCard } from './TmdbCard';
import { UserDetailCard } from './UserDetailCard';

export const GeneralSettings = () => {
  return (
    <Grid container spacing={2}>
      <Grid item xs={12} md={6} lg={4} container direction="column" spacing={2}>
        <Grid item>
          <UserDetailCard delay={200} />
        </Grid>
        <Grid item>
          <LanguageCard delay={225} />
        </Grid>
      </Grid>
      <Grid item xs={12} md={6} lg={4}>
        <RollbarCard delay={250} />
      </Grid>
      <Grid item xs={12} md={6} lg={4}>
        <TmdbCard delay={275} />
      </Grid>
    </Grid>
  )
}
