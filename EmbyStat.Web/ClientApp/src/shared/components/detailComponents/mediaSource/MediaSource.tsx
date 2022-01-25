import { Grid } from '@material-ui/core';
import React from 'react'
import QueryBuilderRoundedIcon from '@material-ui/icons/QueryBuilderRounded';
import StorageRoundedIcon from '@material-ui/icons/StorageRounded';

import { MediaSource as Source } from '../../../models/common/Media';
import calculateFileSize from '../../../../shared/utils/CalculateFileSize';
import calculateRunTime from '../../../../shared/utils/CalculateRunTime';

interface Props {
  source: Source
}

export const MediaSource = (props: Props) => {
  const { source } = props;
  return (
    <Grid item container>
      <Grid item>

      </Grid>
      <Grid item>
        <Grid container direction="column" spacing={1}>
          <Grid item container spacing={1}>
            <Grid item>
              <StorageRoundedIcon />
            </Grid>
            <Grid item>
              {calculateFileSize(source.sizeInMb)}
            </Grid>
          </Grid>
          <Grid item container spacing={1}>
            <Grid item>
              <QueryBuilderRoundedIcon />
            </Grid>
            <Grid item>
              {calculateRunTime(source.runTimeTicks)}
            </Grid>
          </Grid>
        </Grid>
      </Grid>
    </Grid>
  )
}