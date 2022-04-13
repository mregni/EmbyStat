import {Grid} from '@mui/material';
import React from 'react';
import {JobList, JobLogs} from '.';

export const Jobs = () => {
  return (
    <Grid container spacing={2}>
      <Grid item xs={12} lg={6} xl={4}>
        <JobList />
      </Grid>
      <Grid item xs={12} lg={6} xl={8}>
        <JobLogs />
      </Grid>
    </Grid>);
};
