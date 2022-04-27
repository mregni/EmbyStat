import {formatDistanceToNow} from 'date-fns';
import React, {useContext, useEffect, useState} from 'react';
import {useTranslation} from 'react-i18next';

import PlayArrowIcon from '@mui/icons-material/PlayArrow';
import SettingsIcon from '@mui/icons-material/Settings';
import {Box, IconButton, LinearProgress, Paper, Stack, Typography, Zoom} from '@mui/material';

import {EsRoundIconButton} from '../../shared/components/buttons';
import {JobsContext} from '../../shared/context/jobs';
import {useServerType} from '../../shared/hooks';
import {Job} from '../../shared/models/jobs';
import {JobSettingsDialog} from './JobSettingsDialog';

type Props = {
  job: Job;
  i: number;
};

type JobMenuProps = {
  job: Job;
};

function JobMenu(props: JobMenuProps) {
  const {job} = props;
  const [openSettings, setOpenSettings] = useState(false);

  return (
    <>
      <IconButton
        size="small"
        className="m-t-8 m-r-4"
        onClick={() => setOpenSettings(true)}
      >
        <SettingsIcon />
      </IconButton>
      {openSettings ?
        (<JobSettingsDialog
          openSettingsDialog={openSettings}
          job={job}
          onClose={() => setOpenSettings(false)}/>) :
        null}
    </>
  );
}

export function JobItem(props: Props) {
  const {job, i} = props;
  const {t} = useTranslation();
  const [loading, setLoading] = useState(false);
  const {fireJobById} = useContext(JobsContext);
  const {serverType} = useServerType();

  useEffect(() => {
    setLoading([1, 4].includes(job.state));
  }, [job.state]);


  const fireJobAction = () => {
    fireJobById(job.id);
  };

  const jobStateSwitch = (job: Job) => {
    switch (job.state) {
    case 0:
      return t('JOB.NORUN');
    case 1:
      return t('JOB.PROCESSING');
    case 2:
    case 3: {
      const distance = formatDistanceToNow(new Date(job.endTimeUtcIso), {addSuffix: true, includeSeconds: true});
      return `${t('JOB.LASTRUN')} ${distance}`;
    }
    default:
      return '';
    }
  };

  return (
    <Zoom in style={{transitionDelay: `${25 * i + 100}ms`}}>
      <Paper sx={{p: 1}}>
        <Stack direction="row" spacing={2}>
          <Box>
            <EsRoundIconButton
              onClick={fireJobAction}
              Icon={<PlayArrowIcon />}
              disabled={loading}
            />
          </Box>
          <Box sx={{
            display: 'flex',
            flexDirection: 'column',
            flex: 1,
            justifyContent: 'center',
          }}>
            <Box sx={{
              display: 'flex',
              flex: '0 1 auto',
              justifyContent: 'space-between',
            }}>
              <Typography variant="h5">
                {t(`JOB.INFO.${job.title}`, {type: serverType})}
              </Typography>
              <JobMenu job={job} />
            </Box>
            <Box sx={{
              display: 'flex',
              flex: '1 1 auto',
              alignItems: 'center',
              justifyContent: 'space-between',
              paddingRight: 1,
            }}>
              {
                !loading &&
                <Typography variant="body2">
                  {jobStateSwitch(job)}
                </Typography>
              }
              {
                loading &&
                <>
                  <LinearProgress
                    sx={{
                      height: 4,
                      width: 'calc(100% - 45px)',
                    }}
                    color="primary"
                    variant={job.state === 1 ? 'determinate' : 'indeterminate'}
                    value={job.currentProgressPercentage}
                  />
                  <Typography variant="body2">
                    {job.state === 1 ? job.currentProgressPercentage : 0} %
                  </Typography>
                </>
              }
            </Box>
          </Box>
        </Stack>
      </Paper>
    </Zoom>
  );
}
