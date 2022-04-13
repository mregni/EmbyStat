import {CircularProgress, Grid, Paper} from '@mui/material';
import React, {ReactNode} from 'react';

type Props = {
  Icon: ReactNode;
  onClick: any;
  loading?: boolean;
  disabled: boolean;
}

export const EsRoundIconButton = (props: Props) => {
  const {Icon, onClick, loading, disabled} = props;

  const clickedButton = () => {
    if (!disabled) {
      onClick();
    }
  };

  return (
    <Paper
      sx={{
        'borderRadius': '50%',
        'width': 48,
        'height': 48,
        'paddingTop': (theme) => `calc(${theme.spacing(1)} / 2)`,
        'backgroundColor': (theme) => disabled ? '#C0C0C0' : theme.palette.primary.main,
        '&:hover': {
          cursor: !disabled ? 'pointer' : 'default',
        },
        '&:active': {
          boxShadow: (theme) => disabled ? theme.shadows[2] : theme.shadows[0],
        },
      }}
      elevation={10}
      onClick={clickedButton}
    >
      {loading ? (
        <Grid
          container
          justifyContent="center"
          alignItems="center"
          sx={{mt: (theme) => theme.spacing(1)}}
        >
          <CircularProgress size={30} />
        </Grid>
      ) : (
        <Grid
          container
          justifyContent="center"
          alignItems="center"
          sx={{
            '& svg': {
              color: (theme) => theme.palette.mode === 'dark' ? '#222222' : '#DDDDDD',
              width: 40,
              height: 40,
            },
          }}
        >
          {Icon}
        </Grid>
      )}
    </Paper>
  );
};
