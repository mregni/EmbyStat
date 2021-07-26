import React, { useState } from 'react'
import Grid from '@material-ui/core/Grid';
import Button from '@material-ui/core/Button';
import CircularProgress from '@material-ui/core/CircularProgress';
import { makeStyles } from '@material-ui/core/styles';
import { useTranslation } from 'react-i18next';
import { useHistory, useLocation } from 'react-router';
import { useForm } from 'react-hook-form';
import classNames from "classnames";
import { animated } from 'react-spring';

import { LoginView } from '../../shared/models/login';
import { login } from '../../shared/services/AccountService';
import { EsTextInput } from '../../shared/components/esTextInput';
import { EsButton } from '../../shared/components/buttons/EsButton';

const useStyles = makeStyles((theme) => ({
  card__content: {
    height: '100%',
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
  login__button: {
    color: "#d3d3d3",
  },
  "forgot-password__button": {
    color: theme.palette.primary.main,
    fontSize: '0.8rem',
    textDecoration: 'underline',
    cursor: 'pointer',
  }
}));

interface Props {
  style: any,
  openForgotPasswordForm: () => void,
};

export const LoginForm = (props: Props) => {
  const { style, openForgotPasswordForm } = props;
  const classes = useStyles();
  const history = useHistory();
  const location = useLocation();
  const { t } = useTranslation();
  const [failedLogin, setFailedLogin] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [loginView, setLoginView] = useState<LoginView>({
    username: '',
    password: '',
  });
  const referer = location || '/';

  const usernameChanged = (value: string) => {
    setLoginView((prev) => ({
      ...prev,
      username: value,
    }));
  };

  const passwordChanged = (value: string) => {
    setLoginView((prev) => ({
      ...prev,
      password: value,
    }));
  };

  const loginUser = async () => {
    setFailedLogin(false);
    setIsLoading(true);
    const result = await login(loginView);
    if (result) {
      history.push(referer.pathname, { referer: { pathname: '/login' } });
    }

    setIsLoading(false);
    setFailedLogin(true);
  };

  const { register, getValues } = useForm({
    mode: 'onBlur',
    defaultValues: {
      username: '',
      password: '',
    },
  });

  const usernameRegister = register('username');
  const passwordRegister = register('password');

  return (
    <animated.div style={{ ...style }}>
      <Grid container direction="column" justify="flex-end" alignItems="center" spacing={0} className={classes.card__content}>
        <Grid item container>
          <EsTextInput
            inputRef={usernameRegister}
            label={t('SETTINGS.ACCOUNT.USERNAME')}
            onChange={usernameChanged}
            defaultValue={getValues('username')}
            className="p-b-8"
          />
        </Grid>
        <Grid item container className={classNames({ [classes.input__padding]: !failedLogin })}>
          <EsTextInput
            inputRef={passwordRegister}
            label={t('SETTINGS.ACCOUNT.PASSWORD')}
            onChange={passwordChanged}
            defaultValue={getValues('password')}
            type="password"
          />
        </Grid>
        {
          failedLogin ? <Grid item className={classes.error_message}>{t('LOGIN.ERROR')}</Grid> : null
        }
        <Grid item className="p-b-8">
          <Button variant="text" onClick={() => openForgotPasswordForm()} className={classes["forgot-password__button"]}>
            {t('LOGIN.FORGOTPASSWORD')}
          </Button>
        </Grid>
        <Grid item container>
          <EsButton
            onClick={loginUser}
            disabled={isLoading}
          >
            {
              isLoading
                ? <CircularProgress size={21} className={classes.login__button} />
                : t('LOGIN.LOGIN')
            }
          </EsButton>
        </Grid>
      </Grid>
    </animated.div>
  )
}
