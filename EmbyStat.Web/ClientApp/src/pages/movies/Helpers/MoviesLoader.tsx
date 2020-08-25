import React, { ReactElement, ReactNode, useState, useEffect } from 'react';
import { useDispatch, useSelector } from 'react-redux';

import { RootState } from '../../../store/RootReducer';
import { loadStatistics } from '../../../store/MovieSlice';
import { isTypePresent } from '../../../shared/services/MovieService';
import StatisticsLoader from '../../../shared/components/statisticsLoader';
import { getMediaSyncJob } from '../../../shared/services/JobService';

interface Props {
  Component: ReactNode;
}

const MoviesLoader = (props: Props): ReactElement => {
  const { Component } = props;
  const statistics = useSelector((state: RootState) => state.movies);
  const [typePresent, setTypePresent] = useState(false);
  const [typePresentLoading, setTypePresentLoading] = useState(true);
  const [runningSync, setRunningSync] = useState(false);
  const [runningSyncLoading, setRunningSyncLoading] = useState(true);
  const [isLoading, setIsLoading] = useState(!statistics.isLoaded);
  const dispatch = useDispatch();

  useEffect(() => {
    const checkType = async () => {
      const result = await isTypePresent();
      setTypePresent(result);
      setTypePresentLoading(false);
    }

    const checkRunningSync = async () => {
      const result = await getMediaSyncJob();
      setRunningSync(result.state === 1);
      setRunningSyncLoading(false);
    }

    if (!statistics.isLoaded) {
      dispatch(loadStatistics());
    }

    checkRunningSync();
    checkType();

    setIsLoading(!statistics.isLoaded);
  }, [statistics.isLoaded, dispatch]);

  return (
    <StatisticsLoader
      Component={Component}
      noMediaTypeTitle="DIALOGS.NOMOVIETYPEFOUND.TITLE"
      noMediaTypeBody="DIALOGS.NOMOVIETYPEFOUND.BODY"
      typePresent={typePresent}
      typePresentLoading={typePresentLoading}
      runningSync={runningSync}
      runningSyncLoading={runningSyncLoading}
      isLoading={isLoading}
      statistics={statistics.statistics}
      label="MOVIES.LOADER"
    />
  );
};

export default MoviesLoader;
