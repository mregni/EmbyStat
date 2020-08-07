import React from 'react'
import Grid from '@material-ui/core/Grid';
import { makeStyles } from '@material-ui/core/styles';

import LogoGif from '../../assets/images/running-logo.gif';

const useStyles = makeStyles((theme) => ({
  root: {
    display: 'flex',
    justifyContent: 'center',
    alignItems: 'center',
    width: '100vw',
    height: '100vh',
  },
  card: {
    width: 325,
    height: 350,
    padding: 15,
  },
  card__content: {
    height: '100%',
  },
  logo: {
    marginLeft: 10,
  }
}));

interface Props {

}

const PageLoader = (props: Props) => {
  const classes = useStyles();

  return (
    <Grid
      container
      justify="center"
      alignItems="center"
      className={classes.root}>
      <img src={LogoGif} alt="running logo" width="500" />
    </Grid>
  )
}

export default PageLoader
