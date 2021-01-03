import React, { useState } from 'react';
import Grid from '@material-ui/core/Grid';
import Card from '@material-ui/core/Card';
import CardContent from '@material-ui/core/CardContent';
import { makeStyles } from '@material-ui/core/styles';
import { useTransition, config } from 'react-spring';

import LoginForm from './LoginForm';
import RecoverPasswordForm from './RecoverPasswordForm';
import SmallLogo from '../../shared/assets/images/logo-small.png';

const useStyles = makeStyles((theme) => ({
  root: {
    width: '100%',
    height: '100%',
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

const Login = () => {
  const classes = useStyles();
  const [index, openPage] = useState(0);
  const pages = [
    ({ style }) => <LoginForm style={{ ...style, height: '235px', width: '263px' }} openForgotPasswordForm={() => openPage(1)} />,
    ({ style }) => <RecoverPasswordForm style={{ ...style, height: '235px', width: '263px' }} openLoginForm={() => openPage(0)} />,
  ];

  const transitions = useTransition(index, p => p, {
    from: { position: 'absolute', opacity: 0 },
    enter: { opacity: 1 },
    leave: { opacity: 0 },
    config: config.default,
  });

  return (
    <Grid
      container
      justify="center"
      alignItems="center"
      className={classes.root}>
      <Card className={classes.card}>
        <CardContent className={classes.card__content}>
          <Grid container direction="column" justify="space-between">
            <Grid item>
              <img src={SmallLogo} width={250} alt="EmbyStat logo" className={classes.logo} />
            </Grid>
            <Grid item className={classes.card__content}>
              {transitions.map(({ item, props, key }) => {
                const Page = pages[item]
                return <Page key={key} style={props} />
              })}
            </Grid>
          </Grid>
        </CardContent>
      </Card>
    </Grid>
  );
};

export default Login
