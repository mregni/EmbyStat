import React, { useState } from 'react'
import { animated } from 'react-spring';
import Grid from '@material-ui/core/Grid';
import TextField from '@material-ui/core/TextField';
import Button from '@material-ui/core/Button';
import CircularProgress from '@material-ui/core/CircularProgress';
import { makeStyles } from '@material-ui/core/styles';
import { useTranslation } from 'react-i18next';
import { withRouter, RouteComponentProps } from 'react-router-dom';
import { StaticContext } from 'react-router';
import { useForm } from 'react-hook-form';
import classNames from "classnames";

import { LoginView } from '../../shared/models/login';
import { login } from '../../shared/services/AccountService';

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
  button: {
    marginTop: 15,
    height: 36,
    width: '100%',
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

type Props = RouteComponentProps<
  {},
  StaticContext,
  { referer: { pathname: string } }
> & {
  style: any,
  openForgotPasswordForm: () => void,
};

const LoginForm = (props: Props) => {
  const { style, history, openForgotPasswordForm } = props;
  const classes = useStyles();
  const { t } = useTranslation();
  const [failedLogin, setFailedLogin] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [loginView, setLoginView] = useState<LoginView>({
    username: '',
    password: '',
  });
  const { referer } = props.location.state || { referer: { pathname: '/' } };

  const usernameChanged = (event) => {
    event.persist();
    setLoginView((state) => ({
      ...state,
      username: event.target.value as string,
    }));
  };

  const passwordChanged = (event) => {
    event.persist();
    setLoginView((state) => ({
      ...state,
      password: event.target.value as string,
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

  const { register } = useForm({
    mode: 'onBlur',
    defaultValues: {
      username: '',
      password: '',
    },
  });

  return (
    <animated.div style={{ ...style }}>
      <Grid container direction="column" justify="flex-end" spacing={1} className={classes.card__content}>
        <form>
          <Grid item>
            <TextField
              inputRef={register()}
              label={t('SETTINGS.GENERAL.USERNAME')}
              onChange={usernameChanged}
              value={loginView.username}
              color="primary"
              size="small"
              name="username"
              className="p-b-8" />
          </Grid>
          <Grid item className={classNames({ [classes.input__padding]: !failedLogin })}>
            <TextField
              inputRef={register()}
              label={t('SETTINGS.GENERAL.PASSWORD')}
              type="password"
              size="small"
              color="primary"
              name="password"
              onChange={passwordChanged}
              value={loginView.password} />
          </Grid>
          {
            failedLogin ? <Grid item className={classes.error_message}>{t('LOGIN.ERROR')}</Grid> : null
          }
          <Grid item container justify="center">
            <Button variant="text" onClick={() => openForgotPasswordForm()} className={classes["forgot-password__button"]}>
              {t('LOGIN.FORGOTPASSWORD')}
            </Button>
          </Grid>
          <Button
            variant="contained"
            onClick={loginUser}
            className={classes.button}
            disabled={isLoading}
            color="primary">
            {
              isLoading
                ? <CircularProgress size={16} className={classes.login__button} />
                : t('LOGIN.LOGIN')
            }
          </Button>
        </form>
      </Grid>
    </animated.div>
  )
}

export default withRouter(LoginForm);
