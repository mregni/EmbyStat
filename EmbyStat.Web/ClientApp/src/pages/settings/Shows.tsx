import React from 'react';

import {Box, Grid, Stack} from '@mui/material';

import {EsLibrarySelector} from '../../shared/components/esLibrarySelector';
import {EsTitle} from '../../shared/components/esTitle';
import {fetchLibraries, pushLibraries} from '../../shared/services/showService';

export function Shows() {
  return (
    <Stack spacing={2}>
      <EsTitle content='SETTINGS.SHOWS.TITLE' isFirst={true} />
      <Box>
        <Grid container={true} spacing={2}>
          <Grid item={true} xs={12} lg={6} xl={4}>
            <EsLibrarySelector
              fetch={fetchLibraries}
              push={pushLibraries}
              type='COMMON.SHOW'
            />
          </Grid>
        </Grid>
      </Box>
    </Stack>
  );
}
