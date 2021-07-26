import React, { ReactElement, ReactNode, useContext } from 'react';
import { useTranslation } from 'react-i18next';
import { makeStyles } from '@material-ui/core/styles';
import { useSelector } from 'react-redux';
import Grid from '@material-ui/core/Grid';
import Card from '@material-ui/core/Card';
import CardContent from '@material-ui/core/CardContent';
import Typography from '@material-ui/core/Typography';
import Button from '@material-ui/core/Button';
import { useHistory } from 'react-router-dom'

import { Loading } from '../../components/loading';
import { RootState } from '../../../store/RootReducer';
import getMediaServerTypeString from '../../utils/GetMediaServerTypeString';
import { fireJob } from '../../services/JobService';
import { MovieStatistics } from '../../models/movie';
import { ShowStatistics } from '../../models/show';
import { SettingsContext } from '../../context/settings';

const useStyles = makeStyles((theme) => ({
  'full-height': {
    height: '100%',
  },
  root: {
    display: 'flex',
    justifyContent: 'center',
    alignItems: 'center',
    width: '100%',
    height: '100%',
  },
  card: {
    width: 550,
    height: 250,
  },
}));

interface Props {
  children: ReactElement;
  noMediaTypeTitle: string;
  noMediaTypeBody: string;
  typePresent: boolean;
  typePresentLoading: boolean;
  runningSync: boolean;
  runningSyncLoading: boolean;
  isLoading: boolean;
  label: string;
  jobId: string;
}

const StatisticsLoader = (props: Props) => {
  const {
    children,
    noMediaTypeTitle,
    noMediaTypeBody,
    typePresent,
    typePresentLoading,
    runningSync,
    runningSyncLoading,
    isLoading,
    label,
    jobId
  } = props;
  const history = useHistory();
  const classes = useStyles();
  const { t } = useTranslation();
  const { settings } = useContext(SettingsContext);

  const startSync = async () => {
    await fireJob(jobId);
    history.push('/jobs');
  }

  if (!typePresentLoading && !runningSyncLoading && runningSync) {
    return (
      <div
        className={classes.root}>
        <Card className={classes.card}>
          <CardContent className="max-height">
            <Grid container direction="column" justify="space-between" className="max-height">
              <Grid item container direction="column">
                <Grid item>
                  <Typography variant="h5" color="primary" gutterBottom>
                    {t('DIALOGS.MEDIASYNCTASKRUNNING.TITLE')}
                  </Typography>
                </Grid>
                <Grid item>
                  {t('DIALOGS.MEDIASYNCTASKRUNNING.BODY', { type: getMediaServerTypeString(settings) })}
                </Grid>
              </Grid>
            </Grid>
          </CardContent>
        </Card>
      </div>
    )
  }

  if (!runningSyncLoading && !typePresentLoading && !typePresent) {
    return (
      <div
        className={classes.root}>
        <Card className={classes.card}>
          <CardContent className="max-height">
            <Grid container direction="column" justify="space-between" className="max-height">
              <Grid item container direction="column">
                <Grid item>
                  <Typography variant="h5" color="primary" gutterBottom>
                    {t(noMediaTypeTitle)}
                  </Typography>
                </Grid>
                <Grid item>
                  {t(noMediaTypeBody, { type: getMediaServerTypeString(settings) })}
                </Grid>
              </Grid>
              <Grid item container justify="flex-end">
                <Button variant="contained" color="primary" onClick={startSync}>
                  {t('JOB.STARTMEDIASYNC')}
                </Button>
              </Grid>
            </Grid>
          </CardContent>
        </Card>
      </div>
    )
  }

  return (
    <Loading
      loading={isLoading || typePresentLoading || runningSyncLoading}
      label={t(label)}
      className={classes['full-height']}
    >
      {children}
    </Loading>
  );
}

export default StatisticsLoader
