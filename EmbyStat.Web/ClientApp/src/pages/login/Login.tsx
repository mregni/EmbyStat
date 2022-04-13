import {Grid, Card, CardContent} from '@mui/material';
import React, {useState, useEffect, useContext} from 'react';
import {Navigate} from 'react-router';

import {LoginForm, RecoverPasswordForm} from '.';
import SmallLogo from '../../shared/assets/images/logo-small.png';
import {UserContext} from '../../shared/context/user';

export const Login = () => {
  const [inRecoveryMode, setInRecoveryMode] = useState(false);
  const [isLoggedIn, setIsLoggedIn] = useState(false);
  const userContext = useContext(UserContext);

  useEffect(() => {
    (async () => {
      const result = await userContext.isUserLoggedIn();
      setIsLoggedIn(result);
    })();
  }, []);

  if (isLoggedIn) {
    return <Navigate to="/" replace />;
  };

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
};
