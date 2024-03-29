import React from 'react';

import {Box, Grid, Stack} from '@mui/material';

import {EsLibrarySelector} from '../../shared/components/esLibrarySelector';
import {EsTitle} from '../../shared/components/esTitle';
import {fetchLibraries, pushLibraries} from '../../shared/services/movieService';

export function Movies() {
  return (
    <Stack spacing={2}>
      <EsTitle content='SETTINGS.MOVIES.TITLE' isFirst />
      <Box>
        <Grid container spacing={2}>
          <Grid item xs={12} lg={6} xl={4}>
            <EsLibrarySelector
              fetch={fetchLibraries}
              push={pushLibraries}
              type='COMMON.MOVIE'
            />
          </Grid>
        </Grid>
      </Box>
    </Stack>
  );
}
