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
            container={true}
            direction="column"
            spacing={2}>
            <Grid
              item={true}
              spacing={1}
              container={true}
              direction="column"
            >
              <Grid
                item={true}
                spacing={2}
                direction="row"
                container={true}
                alignItems="center"
              >
                <Grid item={true}>
                  <Skeleton variant="text" width={200} height={30} animation="wave" />
                </Grid>
                <Grid item={true}>
                  <Skeleton variant="text" width={120} height={30} animation="wave" />
                </Grid>
              </Grid>
              <Grid item={true}>
                <Stack spacing={1} direction="row">
                  <Skeleton variant="text" width={60} height={24} animation="wave" />
                  <Skeleton variant="text" width={60} height={24} animation="wave" />
                  <Skeleton variant="text" width={60} height={24} animation="wave" />
                </Stack>
              </Grid>
            </Grid>
            <Grid item={true}>
              <Stack
                direction="row"
                spacing={2}
              >
                <Box sx={{minWidth: 200, borderRight: 'solid 1px #aaaaaa'}}>
                  <Grid container={true} direction="column" >
                    <Skeleton variant="text" width={180} height={24} animation="wave" />
                    <Skeleton variant="text" width={60} height={24} animation="wave" style={{marginTop: 2}} />
                    <Skeleton variant="text" width={180} height={24} animation="wave" style={{marginTop: 2}} />
                    <Skeleton variant="text" width={100} height={24} animation="wave" style={{marginTop: 2}} />
                    <Skeleton variant="text" width={100} height={24} animation="wave" style={{marginTop: 2}} />
                  </Grid>
                </Box>
                <Box>
                  <Grid
                    container={true}
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
