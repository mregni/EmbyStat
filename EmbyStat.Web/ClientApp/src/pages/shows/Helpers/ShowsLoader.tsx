import React, { useState, useEffect } from 'react';
import { useDispatch, useSelector } from 'react-redux';

import { RootState } from '../../../store/RootReducer';
import { loadStatistics } from '../../../store/ShowSlice';
import { isTypePresent } from '../../../shared/services/ShowService';
import StatisticsLoader from '../../../shared/components/statisticsLoader';
import { getJobById } from '../../../shared/services/JobService';
import { showJobId } from '../../../shared/utils';

interface Props {
  Component: Function;
}

export const ShowsLoader = (props: Props) => {
  const { Component } = props;
  const statistics = useSelector((state: RootState) => state.shows);
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
      const result = await getJobById(showJobId);
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
      noMediaTypeTitle="DIALOGS.NOSHOWTYPEFOUND.TITLE"
      noMediaTypeBody="DIALOGS.NOSHOWTYPEFOUND.BODY"
      typePresent={typePresent}
      typePresentLoading={typePresentLoading}
      runningSync={runningSync}
      runningSyncLoading={runningSyncLoading}
      isLoading={isLoading}
      label="SHOWS.LOADER"
      jobId={showJobId}
    >
      <Component statistics={statistics.statistics} />
    </StatisticsLoader>
  );
};
