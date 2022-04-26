import React, {useState} from 'react';
import {useForm} from 'react-hook-form';
import {useTranslation} from 'react-i18next';

import {Button, CircularProgress, Grid, Typography} from '@mui/material';

import {EsTextInput} from '../../shared/components/esTextInput';
import {resetPassword} from '../../shared/services/accountService';
import SnackbarUtils from '../../shared/utils/SnackbarUtilsConfigurator';

interface Props {
  openLoginForm: () => void;
}

export function RecoverPasswordForm(props: Props) {
  const {openLoginForm} = props;
  const {t} = useTranslation();
  const [isLoading, setIsLoading] = useState(false);
  const [username, setUsername] = useState<string>('');
  const [resetFailed, setResetFailed] = useState(false);

  const usernameChanged = (value: string) => {
    setUsername(value);
  };

  const {register, getValues} = useForm({
    mode: 'onBlur',
    defaultValues: {
      username: '',
    },
  });

  const recoverPassword = () => {
    setIsLoading(true);
    resetPassword(username)
      .then(((result) => {
        setResetFailed(!result);
        if (result) {
          SnackbarUtils.success(t('LOGIN.RESETCOMPLETED'));
          openLoginForm();
        }
      }))
      .finally(() => {
        setIsLoading(false);
      })
      .catch(() => setIsLoading(false));
  };

  const usernameRegister = register('username');

  return (
    <form>
      <Grid
        container={true}
        direction="column"
        spacing={4}
      >
        <Grid item={true}>
          <Grid container={true} direction="column">
            <Grid item={true}>
              <EsTextInput
                inputRef={usernameRegister}
                label={t('SETTINGS.ACCOUNT.USERNAME')}
                onChange={usernameChanged}
                defaultValue={getValues('username')}
              />
            </Grid>
          </Grid>
        </Grid>

        <Grid item={true}>
          <Grid container={true} spacing={1} direction="column" alignItems="center">
            <Grid item={true}
              sx={{
                marginBottom: resetFailed ? 0 : '23px',
              }}>
              <Typography variant="body1" color="error">{resetFailed ? t('LOGIN.RESETFAILED') : null}</Typography>
            </Grid>
            <Grid item={true} container={true} direction="row" justifyContent="space-between">
              <Grid item={true}>
                <Button
                  variant="text"
                  onClick={openLoginForm}
                  disabled={isLoading}
                  color="secondary"
                >
                  {t('COMMON.BACK')}
                </Button>
              </Grid>
              <Grid item={true}>
                <Button
                  variant="contained"
                  onClick={recoverPassword}
                  disabled={isLoading || username === ''}
                  color="primary">
                  {
                    isLoading ?
                      <CircularProgress size={16} sx={{color: '#d3d3d3'}} /> :
                      t('LOGIN.RECOVERPASSWORD')
                  }
                </Button>
              </Grid>
            </Grid>
          </Grid>
        </Grid>
      </Grid>
    </form>
  );
}
