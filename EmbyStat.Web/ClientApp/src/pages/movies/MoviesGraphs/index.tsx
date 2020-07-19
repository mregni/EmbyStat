import React from 'react';
import Grid from '@material-ui/core/Grid';
import Paper from '@material-ui/core/Paper';
import uuid from 'react-uuid';

import { MovieStatistics } from '../../../shared/models/movie';
import BarGraph from '../../../shared/components/graphs/BarGraph';

interface Props {
  statistics: MovieStatistics;
}

interface Bar {
  label: string;
  [key: number]: any;
}

const MoviesGraphs = (props: Props) => {
  const { statistics } = props;

  return (
    <Grid container alignItems="flex-start" spacing={3}>
      {statistics.charts.map((chart) => {
        return (
          <Grid item xs={12} xl={6} className="m-t-32" key={uuid()}>
            <Paper elevation={5} className="p-16">
              <BarGraph chart={chart} />
            </Paper>
          </Grid>
        );
      })}
    </Grid>
  );
};

export default MoviesGraphs;
