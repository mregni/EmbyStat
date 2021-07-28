import React, { useState } from 'react';
import Grid from '@material-ui/core/Grid';
import Card from '@material-ui/core/Card';
import CardContent from '@material-ui/core/CardContent';
import { makeStyles } from '@material-ui/core/styles';
import { useTransition, config } from 'react-spring';

import { LoginForm, RecoverPasswordForm } from '.';
import SmallLogo from '../../shared/assets/images/logo-small.png';

const useStyles = makeStyles((theme) => ({
  root: {
    display: 'flex',
    justifyContent: 'center',
    alignItems: 'center',
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

export const Login = () => {
  const classes = useStyles();
  const [inRecoveryMode, setInRecoveryMode] = useState(false);

  const transitions = useTransition(inRecoveryMode, {
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
              {transitions((style, item) =>
                item
                  ? <RecoverPasswordForm style={{ ...style, height: '235px', width: '263px' }} openLoginForm={() => setInRecoveryMode((prev) => !prev)} />
                  : <LoginForm style={{ ...style, height: '235px', width: '263px' }} openForgotPasswordForm={() => setInRecoveryMode((prev) => !prev)} />
              )}
            </Grid>
          </Grid>
        </CardContent>
      </Card>
    </Grid>
  );
};
