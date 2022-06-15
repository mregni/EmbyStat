import React, {useMemo} from 'react';

import {Grid, LinearProgress, useTheme} from '@mui/material';
import {orange, yellow} from '@mui/material/colors';

import {ShowRow} from '../../../shared/models/show';

type EpisodeColumnProps = {
  row: ShowRow;
}

export function EpisodeColumn(props: EpisodeColumnProps) {
  const {row} = props;
  const theme = useTheme();
  let value = `${row.episodeCount} / ${row.episodeCount + row.missingEpisodeCount}`;
  if (row.specialEpisodeCount > 0) {
    value += ` +${row.specialEpisodeCount}`;
  }

  const percentage = row.episodeCount / (row.episodeCount + row.missingEpisodeCount) * 100;
  const memoizedValue = useMemo(() => {
    let color = theme.palette.error.main;
    if (percentage === 100) {
      color = theme.palette.primary.main;
    } else if (percentage >= 85) {
      color = yellow[600];
    } else if (percentage >= 65) {
      color = orange[600];
    }

    return color;
  }, [percentage]);

  return (
    <Grid container direction="column">
      <Grid item>
        {value}
      </Grid>
      <Grid item sx={{color: memoizedValue}}>
        <LinearProgress
          value={percentage}
          variant="determinate"
          color="inherit"
        />
      </Grid>
    </Grid>
  );
}
