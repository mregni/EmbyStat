import React from 'react';
import Grid from '@material-ui/core/Grid';

import UserDetailCard from './UserDetailCard';
import RollbarCard from './RollbarCard';
import TvdbCard from './TvdbCard';
import LanguageCard from './LanguageCard';

interface Props {

}

const GeneralSettings = (props: Props) => {
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
        <TvdbCard delay={275} />
      </Grid>
    </Grid>
  )
}

export default GeneralSettings
