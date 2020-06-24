import React, { useEffect } from 'react'
import { Grid } from '@material-ui/core'
import { Job } from '../../shared/models/jobs';
import { loadJobs } from '../../store/JobSlice';
import { useDispatch, useSelector } from 'react-redux';
import { RootState } from '../../store/RootReducer';


import JobItem from './JobItem';
import JobLogs from './JobLogs';

const Jobs = () => {
  const dispatch = useDispatch();

  useEffect(() => {
    dispatch(loadJobs());
  }, [dispatch]);
  const jobsContainer = useSelector((state: RootState) => state.jobs);

  return (
    <Grid container direction="row">
      <Grid item container direction="column" justify="flex-start" xs={12} lg={6}>
        {jobsContainer.jobs.map((job: Job, i: number) =>
          <JobItem key={job.id} job={job} i={i} />
        )}
      </Grid>
      <Grid item xs={12} lg={6}>
        <JobLogs />
      </Grid>
    </Grid >
  )
}

export default Jobs
