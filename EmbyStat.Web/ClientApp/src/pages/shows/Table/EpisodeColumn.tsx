import React from 'react';

import {Grid, LinearProgress} from '@mui/material';

import {ShowRow} from '../../../shared/models/show';

type EpisodeColumnProps = {
  row: ShowRow;
}

export function EpisodeColumn(props: EpisodeColumnProps) {
  const {row} = props;
  let value = `${row.episodeCount} / ${row.episodeCount + row.missingEpisodeCount}`;
  if (row.specialEpisodeCount > 0) {
    value += ` +${row.specialEpisodeCount}`;
  }

  const percentage = row.episodeCount / (row.episodeCount + row.missingEpisodeCount) * 100;

  return (
    <Grid container={true} direction="column">
      <Grid item={true}>
        {value}
      </Grid>
      <Grid item={true}>
        <LinearProgress value={percentage} variant="determinate" color={percentage >= 90 ? 'primary' : 'secondary'} />
      </Grid>
    </Grid>
  );
}
