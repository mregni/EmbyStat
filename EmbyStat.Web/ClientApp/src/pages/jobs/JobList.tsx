import {Stack} from '@mui/material';
import React, {useContext} from 'react';
import {JobItem} from '.';
import {JobsContext} from '../../shared/context/jobs';
import {Job} from '../../shared/models/jobs';

export const JobList = () => {
  const {jobs} = useContext(JobsContext);

  return (
    <Stack direction="column" spacing={2}>
      {jobs.map((job: Job, i: number) => (<JobItem key={job.id} job={job} i={i} ></JobItem>))}
    </Stack>
  );
};
