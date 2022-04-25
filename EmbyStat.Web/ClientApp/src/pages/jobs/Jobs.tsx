import {Grid} from '@mui/material';
import React from 'react';
import {JobList, JobLogs} from '.';

export function Jobs() {
  return (
    <Grid container={true} spacing={2}>
      <Grid item={true} xs={12} lg={6} xl={4}>
        <JobList />
      </Grid>
      <Grid item={true} xs={12} lg={6} xl={8}>
        <JobLogs />
      </Grid>
    </Grid>);
}
