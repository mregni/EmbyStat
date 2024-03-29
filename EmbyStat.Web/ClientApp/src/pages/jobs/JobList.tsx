import React, {useContext} from 'react';

import {Stack} from '@mui/material';

import {JobsContext} from '../../shared/context/jobs';
import {Job} from '../../shared/models/jobs';
import {JobItem} from './JobItem';

export function JobList() {
  const {jobs} = useContext(JobsContext);

  return (
    <Stack direction="column" spacing={2}>
      {jobs.map((job: Job, i: number) => (<JobItem key={job.id} job={job} i={i} />))}
    </Stack>
  );
}
