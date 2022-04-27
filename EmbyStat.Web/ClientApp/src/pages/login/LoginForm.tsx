import React, {useContext, useState} from 'react';
import {useForm} from 'react-hook-form';
import {useTranslation} from 'react-i18next';
import {useNavigate} from 'react-router';

import {CircularProgress, Grid, Typography} from '@mui/material';

import {EsButton} from '../../shared/components/buttons';
import {EsTextInput} from '../../shared/components/esTextInput';
import {UserContext} from '../../shared/context/user';
import {useEsLocation} from '../../shared/hooks';

interface Props {
  openForgotPasswordForm: () => void,
};

export function LoginForm(props: Props) {
  const {openForgotPasswordForm} = props;
  const navigate = useNavigate();
  const location = useEsLocation();
  const {t} = useTranslation();
  const [failedLogin, setFailedLogin] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const {login, isUserLoggedIn} = useContext(UserContext);

  const from = location.state?.from?.pathname || '/';

  const loginUser = async () => {
    setFailedLogin(false);
    setIsLoading(true);
    const {username, password} = getValues();
    try {
      await login(username, password);
      if (await isUserLoggedIn()) {
        navigate(from, {replace: true});
        return;
      }
      setFailedLogin(true);
    } catch {
      setFailedLogin(true);
    } finally {
      setIsLoading(false);
    }
  };

  const {register, getValues} = useForm({
    mode: 'onBlur',
    defaultValues: {
      username: '',
      password: '',
    },
  });

  const usernameRegister = register('username');
  const passwordRegister = register('password');

  return (
    <Grid
      container
      direction="column"
      spacing={3}
    >
      <Grid item>
        <Grid container direction="column" spacing={1} >
          <Grid item>
            <EsTextInput
              inputRef={usernameRegister}
              label={t('SETTINGS.ACCOUNT.USERNAME')}
              defaultValue={getValues('username')}
            />
          </Grid>
          <Grid item>
            <EsTextInput
              inputRef={passwordRegister}
              label={t('SETTINGS.PASSWORD.PASSWORD')}
              defaultValue={getValues('password')}
              type="password"
            />
          </Grid>
        </Grid>
      </Grid>
      <Grid item>
        <Grid container spacing={1} direction="column" alignItems="center">
          <Grid item
            sx={{
              marginBottom: failedLogin ? 0 : '23px',
            }}>
            <Typography variant="body1" color="error">{failedLogin ? t('LOGIN.ERROR') : null}</Typography>
          </Grid>
          <Grid item container direction="row" justifyContent="space-between">
            <Grid item>
              <EsButton
                variant="text"
                onClick={() => openForgotPasswordForm()}
                color="secondary"
              >
                {t('LOGIN.RECOVERPASSWORD')}
              </EsButton>
            </Grid>
            <Grid item>
              <EsButton
                onClick={loginUser}
                disabled={isLoading}
              >
                {
                  isLoading ?
                    <CircularProgress size={21} sx={{color: '#d3d3d3'}} /> :
                    t('LOGIN.LOGIN')
                }
              </EsButton>
            </Grid>
          </Grid>
        </Grid>
      </Grid>
    </Grid>
  );
}
