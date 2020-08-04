import React, { ReactNode } from 'react';
import Grid from '@material-ui/core/Grid';
import Paper from '@material-ui/core/Paper';
import CircularProgress from '@material-ui/core/CircularProgress';
import { makeStyles } from '@material-ui/core/styles';
import classNames from 'classnames';

const useStyles = makeStyles((theme) => ({
  button: {
    borderRadius: '50%',
    width: 48,
    height: 48,
    paddingTop: 4,
  },
  button__icon: {
    '& svg': {
      color: theme.palette.type === 'dark' ? '#222222' : '#DDDDDD',
      width: 40,
      height: 40,
    },
  },
  button__enabled: {
    backgroundColor: theme.palette.primary.main,
    '&:hover': {
      cursor: 'pointer',
    },
    '&:active': {
      boxShadow: theme.shadows[2],
    },
  },
  button__disabled: {
    backgroundColor: '#C0C0C0',
  },
  button__progress: {
    marginTop: 5,
  },
}));

interface Props {
  Icon: ReactNode;
  onClick: any;
  loading?: boolean;
  disabled: boolean;
}

const RoundIconButton = (props: Props) => {
  const { Icon, onClick, loading, disabled } = props;
  const classes = useStyles();

  const clickedButton = () => {
    if (!disabled) {
      onClick();
    }
  };

  return (
    <Paper
      className={classNames(
        classes.button,
        { [classes.button__enabled]: !disabled },
        { [classes.button__disabled]: disabled }
      )}
      elevation={10}
      onClick={clickedButton}
    >
      {loading ? (
        <Grid
          container
          justify="center"
          alignItems="center"
          className={classes.button__progress}
        >
          <CircularProgress size={30} />
        </Grid>
      ) : (
          <Grid
            container
            justify="center"
            alignItems="center"
            className={classes.button__icon}
          >
            {Icon}
          </Grid>
        )}
    </Paper>
  );
};

RoundIconButton.defaultProps = {
  loading: false,
};

export default RoundIconButton;
