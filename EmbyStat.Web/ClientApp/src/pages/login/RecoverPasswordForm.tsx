import React, { useState } from 'react'
import { animated } from 'react-spring';
import Grid from '@material-ui/core/Grid';
import Button from '@material-ui/core/Button';
import CircularProgress from '@material-ui/core/CircularProgress';
import { makeStyles } from '@material-ui/core/styles';
import { useTranslation } from 'react-i18next';
import { useForm } from 'react-hook-form';

import { resetPassword } from '../../shared/services/AccountService';
import SnackbarUtils from '../../shared/utils/SnackbarUtilsConfigurator';
import { EsTextInput } from '../../shared/components/esTextInput';

const useStyles = makeStyles((theme) => ({
  card__content: {
    height: '100%',
  },
  button: {
    marginTop: 15,
    height: 36,
    width: '100%'
  },
  input__padding: {
    marginBottom: 30,
  },
  input__root: {
    marginTop: 0,
  },
  error_message: {
    color: theme.palette.error.main,
    fontSize: '0.8rem',
    lineHeight: '0.8rem',
    marginTop: 10,
    height: 20,
  },
  recover__button: {
    color: "#d3d3d3",
  }
}));

interface Props {
  style: any;
  openLoginForm: () => void;
}

export const RecoverPasswordForm = (props: Props) => {
  const { style, openLoginForm } = props;
  const classes = useStyles();
  const { t } = useTranslation();
  const [isLoading, setIsLoading] = useState(false);
  const [username, setUsername] = useState<string>('');

  const usernameChanged = (value: string) => {
    setUsername(value);
  };

  const { register, getValues } = useForm({
    mode: 'onBlur',
    defaultValues: {
      username: '',
    },
  });

  const recoverPassword = () => {
    setIsLoading(true);
    resetPassword(username)
      .then((result => {
        if (result) {
          SnackbarUtils.success(t('LOGIN.RESETCOMPLETED'));
          openLoginForm();
        } else {
          SnackbarUtils.error(t('JOB.RESETFAILED'));
        }
      }))
      .finally(() => {
        setIsLoading(false);
      })
  }

  const usernameRegister = register('username');

  return (
    <animated.div style={{ ...style }}>
      <Grid container direction="column" justify="flex-end" spacing={1} className={classes.card__content}>
        <form>
          <Grid item>
            <EsTextInput
              inputRef={usernameRegister}
              label={t('SETTINGS.ACCOUNT.USERNAME')}
              onChange={usernameChanged}
              defaultValue={getValues('username')}
            />
          </Grid>
          <Grid item xs={12}>
            <Button
              variant="contained"
              onClick={recoverPassword}
              className={classes.button}
              disabled={isLoading || username === ''}
              color="primary">
              {
                isLoading
                  ? <CircularProgress size={16} className={classes.recover__button} />
                  : t('LOGIN.RECOVERPASSWORD')
              }
            </Button>
          </Grid>
          <Grid item xs={12}>
            <Button
              variant="text"
              onClick={openLoginForm}
              className={classes.button}
              disabled={isLoading}
              color="secondary">
              {t('COMMON.BACK')}
            </Button>
          </Grid>
        </form>
      </Grid>
    </animated.div>
  )
}
