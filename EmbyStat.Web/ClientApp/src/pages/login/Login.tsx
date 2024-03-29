import React, {useContext, useEffect, useState} from 'react';
import {Navigate} from 'react-router';

import {Card, CardContent, Grid} from '@mui/material';

import SmallLogo from '../../shared/assets/images/logo-small.png';
import {UserContext} from '../../shared/context/user';
import {LoginForm} from './LoginForm';
import {RecoverPasswordForm} from './RecoverPasswordForm';

export function Login() {
  const [inRecoveryMode, setInRecoveryMode] = useState(false);
  const [isLoggedIn, setIsLoggedIn] = useState(false);
  const {isUserLoggedIn} = useContext(UserContext);

  useEffect(() => {
    (async () => {
      const result = await isUserLoggedIn();
      setIsLoggedIn(result);
    })();
  // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  if (isLoggedIn) {
    return <Navigate to="/" replace />;
  }

  return (
    <Grid
      container
      justifyContent="center"
      alignItems="center"
      sx={{
        width: '100vw',
        height: '100vh',
      }}>
      <Card>
        <CardContent
          sx={{
            paddingTop: '20px',
          }}>
          <Grid
            container
            direction="column"
            justifyContent="space-between"
          >
            <Grid item container justifyContent="center">
              <img src={SmallLogo} width={250} alt="EmbyStat logo" style={{marginBottom: 10}}/>
            </Grid>
            <Grid item>
              {
                inRecoveryMode ?
                  <RecoverPasswordForm openLoginForm={() => setInRecoveryMode((prev) => !prev)} /> :
                  <LoginForm openForgotPasswordForm={() => setInRecoveryMode((prev) => !prev)} />
              }
            </Grid>
          </Grid>
        </CardContent>
      </Card>
    </Grid>
  );
}
