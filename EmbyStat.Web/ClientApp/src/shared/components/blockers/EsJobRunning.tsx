import {Card, CardContent, Stack, Typography, LinearProgress, Box} from '@mui/material';
import {} from 'devextreme-react';
import {t} from 'i18next';
import React, {ReactElement, useContext, useState, useEffect} from 'react';
import {JobsContext} from '../../context/jobs';
import {Job} from '../../models/jobs';

type Props = {
  children: ReactElement;
  title: string;
  body: string;
  jobId: string;
  finishedAction: () => Promise<void>;
}

export const EsJobRunning = (props: Props) => {
  const {children, title, body, jobId, finishedAction} = props;
  const [isRunning, setIsRunning] = useState(false);
  const {jobs, logLines} = useContext(JobsContext);
  const [job, setJob] = useState<Job>(null!);

  useEffect(() => {
    const index = jobs.findIndex((j) => j.id === jobId);
    if (index !== -1) {
      const job = jobs[index];
      setIsRunning([1, 4].includes(job.state));
      setJob(job);
      if ([0, 2, 3].includes(job.state)) {
        (async () => {
          if ([0, 2, 3].includes(job.state)) {
            await finishedAction();
          }
        })();
      }
    }
  }, [jobs]);

  if (!isRunning) {
    return (children);
  }

  return (
    <Box
      sx={{
        display: 'flex',
        height: '100%',
        justifyContent: 'center',
        alignItems: 'center',
      }}>
      <Card
        sx={{
          width: 350,
          height: 200,
        }}
      >
        <CardContent sx={{height: '100%'}}>
          <Stack direction="column" justifyContent="space-between"
            sx={{
              height: '100%',
            }}
          >
            <Box>
              <Typography variant="h5" color="primary" gutterBottom>
                {t(title)}
              </Typography>
              <Typography variant="body1">
                {t(body)}
              </Typography>
            </Box>
            <Box>
              <Typography variant="caption">
                {logLines[logLines.length - 1].value}
              </Typography>
              <LinearProgress variant="determinate" value={job?.currentProgressPercentage ?? 0} />
            </Box>
          </Stack>
        </CardContent>
      </Card>
    </Box>
  );
};
