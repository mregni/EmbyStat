import React from 'react';

import {Box, Grid, Skeleton, Stack} from '@mui/material';

export function EsDetailRowSkeleton() {
  return (
    <Box sx={{width: '100%', p: 2}} >
      <Box sx={{display: 'flex'}}>
        <Box sx={{flexGrow: 0, width: 200}}>
          <Skeleton variant="rectangular" width={200} height={300} animation="wave" />
        </Box>
        <Box sx={{flexGrow: 1, pl: 2}}>
          <Grid
            container
            direction="column"
            spacing={2}>
            <Grid
              item
              spacing={1}
              container
              direction="column"
            >
              <Grid
                item
                spacing={2}
                direction="row"
                container
                alignItems="center"
              >
                <Grid item>
                  <Skeleton variant="text" width={200} height={30} animation="wave" />
                </Grid>
                <Grid item>
                  <Skeleton variant="text" width={120} height={30} animation="wave" />
                </Grid>
              </Grid>
              <Grid item>
                <Stack spacing={1} direction="row">
                  <Skeleton variant="text" width={60} height={24} animation="wave" />
                  <Skeleton variant="text" width={60} height={24} animation="wave" />
                  <Skeleton variant="text" width={60} height={24} animation="wave" />
                </Stack>
              </Grid>
            </Grid>
            <Grid item>
              <Stack
                direction="row"
                spacing={2}
              >
                <Box sx={{minWidth: 200, borderRight: 'solid 1px #aaaaaa'}}>
                  <Grid container direction="column" >
                    <Skeleton variant="text" width={180} height={24} animation="wave" />
                    <Skeleton variant="text" width={60} height={24} animation="wave" style={{marginTop: 2}} />
                    <Skeleton variant="text" width={180} height={24} animation="wave" style={{marginTop: 2}} />
                    <Skeleton variant="text" width={100} height={24} animation="wave" style={{marginTop: 2}} />
                    <Skeleton variant="text" width={100} height={24} animation="wave" style={{marginTop: 2}} />
                  </Grid>
                </Box>
                <Box>
                  <Grid
                    container
                    direction="column"
                    justifyContent="flex-start"
                  >
                    <Skeleton variant="text" width={180} height={24} animation="wave" />
                    <Skeleton variant="text" width={180} height={24} animation="wave" style={{marginTop: 2}} />
                    <Skeleton variant="text" width={300} height={24} animation="wave" style={{marginTop: 2}} />
                    <Skeleton variant="text" width={300} height={24} animation="wave" style={{marginTop: 2}} />
                  </Grid>
                </Box>
              </Stack>
            </Grid>
          </Grid>
        </Box>
      </Box>
    </Box>
  );
}
