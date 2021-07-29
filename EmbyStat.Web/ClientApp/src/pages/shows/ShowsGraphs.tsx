import React from 'react';
import Grid from '@material-ui/core/Grid';
import Paper from '@material-ui/core/Paper';
import uuid from 'react-uuid';

import { ShowStatistics } from '../../shared/models/show';
import BarGraph from '../../shared/components/graphs/BarGraph';
import PieGraph from '../../shared/components/graphs/PieGraph';

interface Props {
  statistics: ShowStatistics;
}

export const ShowsGraphs = (props: Props) => {
  const { statistics } = props;

  return (
    <Grid container alignItems="flex-start" spacing={3}>
      {statistics.barCharts.map((chart) => {
        return (
          <Grid item xs={12} xl={6} key={uuid()}>
            <Paper elevation={5} className="p-16">
              <BarGraph chart={chart} />
            </Paper>
          </Grid>
        );
      })}
      {statistics.pieCharts.map((chart) => {
        return (
          <Grid item xs={12} md={6} xl={3} key={uuid()}>
            <Paper elevation={5} className="p-16">
              <PieGraph chart={chart} />
            </Paper>
          </Grid>
        );
      })}
    </Grid>
  );
};
