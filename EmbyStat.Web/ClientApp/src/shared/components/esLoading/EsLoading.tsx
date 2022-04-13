import React, {ReactElement} from 'react';

import {Grid, LinearProgress, Typography} from '@mui/material';

interface Props {
  children: ReactElement | ReactElement[];
  loading: boolean;
  label: string;
  width?: string | number;
  height?: string | number;
}

export const EsLoading = (props: Props): ReactElement => {
  const {
    children,
    loading,
    label,
    width = 'calc(100vw - 264px)',
    height = 'calc(100vh - 217px)',
  } = props;

  if (loading) {
    return (
      <Grid
        container
        justifyContent="center"
        alignItems="center"
        sx={{
          width: width,
          height: height,
        }}
      >
        <Grid
          item
          container
          direction="column"
          justifyContent="center"
          spacing={2}
          sx={{
            maxWidth: 400,
          }}
        >
          <Grid item>
            <Typography align='center' variant="body1">{label}</Typography>
          </Grid>
          <Grid item>
            <LinearProgress />
          </Grid>
        </Grid>
      </Grid>
    );
  }

  return (<>{children}</>);
};
