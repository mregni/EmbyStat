import React, { ReactElement, useState, useEffect } from 'react';
import { useDispatch, useSelector } from 'react-redux';

import { RootState } from '../../../store/RootReducer';
import { loadStatistics } from '../../../store/MovieSlice';
import { isTypePresent } from '../../../shared/services/MovieService';
import StatisticsLoader from '../../../shared/components/statisticsLoader';
import { getJobById } from '../../../shared/services/JobService';
import { movieJobId } from '../../../shared/utils';

interface Props {
  Component: Function;
}

export const MoviesLoader = (props: Props): ReactElement => {
  const { Component } = props;
  const statistics = useSelector((state: RootState) => state.movies);
  const [typePresent, setTypePresent] = useState(false);
  const [typePresentLoading, setTypePresentLoading] = useState(true);
  const [runningSync, setRunningSync] = useState(true);
  const [runningSyncLoading, setRunningSyncLoading] = useState(true);
  const [isLoading, setIsLoading] = useState(!statistics.isLoaded);
  const dispatch = useDispatch();

  useEffect(() => {
    if (!runningSync && !statistics.isLoaded) {
      console.log(runningSync);
      dispatch(loadStatistics());
    }

    setIsLoading(!statistics.isLoaded);
  }, [dispatch, runningSync, statistics.isLoaded])

  useEffect(() => {
    const checkType = async () => {
      const result = await isTypePresent();
      setTypePresent(result);
      setTypePresentLoading(false);
    }

    const checkRunningSync = async () => {
      const result = await getJobById(movieJobId);
      setRunningSync(result.state === 1);
      setRunningSyncLoading(false);
    }

    checkRunningSync();
    checkType();
  }, [statistics.isLoaded, dispatch]);

  return (
    <StatisticsLoader
      noMediaTypeTitle="DIALOGS.NOMOVIETYPEFOUND.TITLE"
      noMediaTypeBody="DIALOGS.NOMOVIETYPEFOUND.BODY"
      typePresent={typePresent}
      typePresentLoading={typePresentLoading}
      runningSync={runningSync}
      runningSyncLoading={runningSyncLoading}
      isLoading={isLoading}
      label="MOVIES.LOADER"
      jobId={movieJobId}
    >
      <Component statistics={statistics.statistics} />
    </StatisticsLoader>
  );
};
