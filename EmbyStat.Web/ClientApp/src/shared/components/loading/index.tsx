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

const Loading = ({ Component, loading, label, className, ...props }): ReactElement => {
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
  return <Component className={className} {...props} />;
};

Loading.propTypes = {
  Component: PropTypes.func.isRequired,
  loading: PropTypes.bool.isRequired,
  label: PropTypes.string.isRequired,
  className: PropTypes.string,
};

Loading.defaultProps = {
  className: '',
};

export default Loading;
