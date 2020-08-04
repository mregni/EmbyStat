import React, { ReactElement, ReactNode, useState, useEffect } from 'react';
import { useTranslation } from 'react-i18next';
import { makeStyles } from '@material-ui/core/styles';
import { useDispatch, useSelector } from 'react-redux';
import Grid from '@material-ui/core/Grid';
import Card from '@material-ui/core/Card';
import CardContent from '@material-ui/core/CardContent';
import Typography from '@material-ui/core/Typography';
import Button from '@material-ui/core/Button';
import { useHistory } from 'react-router-dom'

import Loading from '../../../shared/components/loading';
import { RootState } from '../../../store/RootReducer';
import { loadStatistics } from '../../../store/MovieSlice';
import { isTypePresent } from '../../../shared/services/MovieService';
import getMediaServerTypeString from '../../../shared/utils/GetMediaServerTypeString';
import { fireJob, getMediaSyncJob } from '../../../shared/services/JobService';

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
    width: 400,
    height: 250,
  },
}));

interface Props {
  Component: ReactNode;
}

const MoviesLoader = (props: Props): ReactElement => {
  const { Component } = props;
  const { t } = useTranslation();
  const history = useHistory();
  const classes = useStyles();
  const statistics = useSelector((state: RootState) => state.movies);
  const settings = useSelector((state: RootState) => state.settings);
  const [typePresent, setTypePresent] = useState(false);
  const [typePresentLoading, setTypePresentLoading] = useState(true);
  const [runningSync, setRunningSync] = useState(false);
  const [runningSyncLoading, setRunningSyncLoading] = useState(true);
  const [isLoading, setIsLoading] = useState(true);
  const dispatch = useDispatch();

  useEffect(() => {
    dispatch(loadStatistics());
  }, [dispatch]);

  useEffect(() => {
    setIsLoading(!statistics.isLoaded);
  }, [statistics.isLoaded]);

  useEffect(() => {
    const checkType = async () => {
      const result = await isTypePresent();;
      setTypePresent(result);
      setTypePresentLoading(false);
    }

    const checkRunningSync = async () => {
      const result = await getMediaSyncJob();
      setRunningSync(result.state === 1);
      setRunningSyncLoading(false);
    }

    checkRunningSync();
    checkType();
  }, []);

  const startSync = async () => {
    await fireJob('be68900b-ee1d-41ef-b12f-60ef3106052e');
    history.push('/jobs');
  }

  if (!runningSyncLoading && runningSync) {
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

  if (!typePresentLoading && !typePresent) {
    return (
      <div
        className={classes.root}>
        <Card className={classes.card}>
          <CardContent className="max-height">
            <Grid container direction="column" justify="space-between" className="max-height">
              <Grid item container direction="column">
                <Grid item>
                  <Typography variant="h5" color="primary" gutterBottom>
                    {t('DIALOGS.NOMOVIETYPEFOUND.TITLE')}
                  </Typography>
                </Grid>
                <Grid item>
                  {t('DIALOGS.NOMOVIETYPEFOUND.BODY', { type: getMediaServerTypeString(settings) })}
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
      Component={Component}
      loading={isLoading || typePresentLoading || runningSyncLoading}
      label={t('MOVIES.LOADER')}
      className={classes['full-height']}
      statistics={statistics.statistics}
    />
  );
};

export default MoviesLoader;
