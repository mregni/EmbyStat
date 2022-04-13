import {Card, CardContent, Typography, Button, CardActions, Box, Stack} from '@mui/material';
import {t} from 'i18next';
import React, {ReactElement, useContext} from 'react';
import {JobsContext} from '../../context/jobs';
import {useServerType} from '../../hooks';

type Props = {
  mediaPresent: boolean;
  children: ReactElement;
  title: string;
  body: string;
  jobId: string;
}

export const EsNoMedia = (props: Props) => {
  const {mediaPresent, children, title, body, jobId} = props;
  const {fireJobById} = useContext(JobsContext);
  const {getMediaServerTypeString} = useServerType();

  if (mediaPresent) {
    return (children);
  }

  const startSync = async () => {
    await fireJobById(jobId);
  };

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
          minHeight: 200,
        }}
      >
        <CardContent>
          <Stack direction="column" justifyContent="space-between" >
            <Box>
              <Typography variant="h5" color="primary" gutterBottom>
                {t(title)}
              </Typography>
              <Typography variant="body1">
                {t(body, {type: getMediaServerTypeString()})}
              </Typography>
            </Box>
          </Stack>
        </CardContent>
        <CardActions sx={{p: 2}}>
          <Button variant="contained" color="primary" onClick={startSync}>
            {t('JOB.STARTMEDIASYNC')}
          </Button>
        </CardActions>
      </Card>
    </Box>
  );
};
