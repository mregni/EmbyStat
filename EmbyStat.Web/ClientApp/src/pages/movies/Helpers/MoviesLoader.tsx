import React, { ReactElement, ReactNode, useState, useEffect } from 'react';
import { useTranslation } from 'react-i18next';
import { makeStyles } from '@material-ui/core/styles';
import { useDispatch, useSelector } from 'react-redux';

import Loading from '../../../shared/components/loading';
import { RootState } from '../../../store/RootReducer';
import { loadStatistics } from '../../../store/MovieSlice';

const useStyles = makeStyles((theme) => ({
  'full-height': {
    height: '100%',
  },
}));

interface Props {
  Component: ReactNode;
}

const MoviesLoader = (props: Props): ReactElement => {
  const { Component } = props;
  const { t } = useTranslation();
  const classes = useStyles();
  const statistics = useSelector((state: RootState) => state.movies);
  const [isLoading, setIsLoading] = useState(true);
  const dispatch = useDispatch();

  useEffect(() => {
    dispatch(loadStatistics());
  }, [dispatch]);

  useEffect(() => {
    setIsLoading(!statistics.isLoaded);
  }, [statistics.isLoaded]);

  return (
    <Loading
      Component={Component}
      loading={isLoading}
      label={t('MOVIES.LOADER')}
      className={classes['full-height']}
      statistics={statistics.statistics}
    />
  );
};

export default MoviesLoader;
