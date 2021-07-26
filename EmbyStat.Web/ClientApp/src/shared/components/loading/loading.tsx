import React, { ReactElement } from 'react';
import Grid from '@material-ui/core/Grid';
import LinearProgress from '@material-ui/core/LinearProgress';
import { makeStyles } from '@material-ui/core/styles';
import PropTypes from 'prop-types';

const useStyles = makeStyles((theme) => ({
  loader: {
    maxWidth: '400px',
  },
}));

interface Props {
  children: ReactElement | ReactElement[];
  loading: boolean;
  label: string;
  className?: string;
}

export const Loading = (props: Props): ReactElement => {
  const { children, loading, label, className = '' } = props;
  const classes = useStyles();
  if (loading) {
    return (
      <Grid
        container
        direction="row"
        justify="center"
        alignItems="center"
        className={className}
      >
        <Grid
          container
          direction="column"
          justify="center"
          className={classes.loader}
        >
          <Grid item container justify="center">
            <p className="m-b-16">{label}</p>
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
