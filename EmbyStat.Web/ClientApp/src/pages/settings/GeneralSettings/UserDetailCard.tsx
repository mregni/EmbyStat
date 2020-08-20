import React, { useState, useEffect } from 'react';
import Grid from '@material-ui/core/Grid';
import Typography from '@material-ui/core/Typography';
import { makeStyles } from '@material-ui/core/styles';
import { useForm } from 'react-hook-form';
import TextField from '@material-ui/core/TextField';
import { useTranslation } from 'react-i18next';
import classNames from 'classnames';

import { getUserInfo, changePassword, changeUserName, login } from '../../../shared/services/AccountService';
import SnackbarUtils from '../../../shared/utils/SnackbarUtilsConfigurator';
import { User } from '../../../shared/models/login';
import SettingsCard from '../SettingsCard';

const useStyles = makeStyles({
  normall__input: {
    paddingBottom: 25,
  },
  error__input: {
    paddingBottom: 2,
  }
});

interface Props {
  delay: number
}

const UserDetailCard = (props: Props) => {
  const { delay } = props;
  const classes = useStyles();
  const [user, setUser] = useState<User | null>(null);
  const { t } = useTranslation();

  useEffect(() => {
    const us = getUserInfo();
    setUser(us);
    setUsername(us?.username ?? '');
  }, []);

  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [confirmedPassword, setConfirmedPassword] = useState('');
  const [currentPassword, setCurrentPassword] = useState('');

  const { register, errors, triggerValidation } = useForm({
    mode: 'onBlur',
    defaultValues: {
      username: username,
      password: '',
      confirmedPassword: '',
      currentPassword: '',
    }
  });

  const getPasswordErrorMessage = (): string => {
    switch (errors.password?.type) {
      case 'required':
        return t('SETTINGS.ACCOUNT.NOPASSWORD');
      case 'minLength':
        return t('SETTINGS.ACCOUNT.PASSWORDMINLENGTH');
      default:
        return '';
    }
  };

  const saveUser = async () => {
    await updateUserName();
    await updatePassword();
  }

  const updateUserName = async () => {
    if (username !== user?.username) {
      const valid = await triggerValidation(['username', 'currentPassword']);
      if (valid) {
        const result = await changeUserName({ userName: user?.username ?? '', newUserName: username });
        if (result) {
          var loginResult = await login({ username, password: currentPassword });
          if (!loginResult) {
            SnackbarUtils.error(t('SETTINGS.ACCOUNT.WRONGPASSWORD'));
            await changeUserName({ userName: username, newUserName: user?.username ?? '' });
            return;
          }

          SnackbarUtils.success(t('SETTINGS.ACCOUNT.USERNAMEUPDATED'));
          return;
        }

        SnackbarUtils.error(t('SETTINGS.ACCOUNT.USERNAMEUPDATEFAILED'));
      }
    }
  }

  const updatePassword = async () => {
    if (password.length !== 0) {
      const valid = await triggerValidation(['confirmedPassword', 'password', 'currentPassword']);
      if (valid) {
        const result = await changePassword(
          {
            userName: username,
            oldPassword: currentPassword,
            newPassword: password
          });
        if (result) {
          await login({ username, password });
          SnackbarUtils.success('Password updated succesfully');
        }
      }
    }
  }

  const isEqual = (value: string) => value === password;

  return (
    <SettingsCard
      delay={delay}
      title={t('SETTINGS.ACCOUNT.TITLE')}
      saveForm={saveUser}
    >
      <Grid item className={classNames({
        [classes.normall__input]: !errors.username,
        [classes.error__input]: errors.username,
      })}>
        <TextField
          inputRef={register({ required: t('SETTINGS.ACCOUNT.NOUSERNAME').toString() })}
          label={t('SETTINGS.ACCOUNT.USERNAME')}
          size="small"
          name="username"
          error={!!errors.username}
          helperText={errors.username ? errors.username.message : ''}
          color="primary"
          value={username}
          onChange={(event) => setUsername(event.target.value as string)}
        />
      </Grid>
      <Grid item className={classNames({
        [classes.normall__input]: !errors.currentPassword,
        [classes.error__input]: errors.currentPassword,
      })}>
        <TextField
          inputRef={register({ required: true })}
          label={t('SETTINGS.ACCOUNT.CURRENTPASSWORD')}
          size="small"
          type="password"
          color="primary"
          name="currentPassword"
          error={!!errors.currentPassword}
          helperText={errors.currentPassword ? t('SETTINGS.ACCOUNT.NOPASSWORD') : ''}
          value={currentPassword}
          onChange={(event) => setCurrentPassword(event.target.value as string)}
        />
      </Grid>
      <Grid item className="m-t-32">
        <Typography variant="body2">
          {t('SETTINGS.ACCOUNT.PASSWORDWARNING')}
        </Typography>
      </Grid>
      <Grid item className={classNames({
        [classes.normall__input]: !errors.password,
        [classes.error__input]: errors.password
      })}>
        <TextField
          inputRef={register({ required: true, minLength: 6 })}
          label={t('SETTINGS.ACCOUNT.NEWPASSWORD')}
          size="small"
          type="password"
          color="primary"
          name="password"
          error={!!errors.password}
          helperText={errors.password ? getPasswordErrorMessage() : ''}
          value={password}
          onChange={(event) => setPassword(event.target.value as string)}
        />
      </Grid>
      <Grid item className={classNames({
        [classes.normall__input]: !errors.confirmedPassword,
        [classes.error__input]: errors.confirmedPassword
      })}>
        <TextField
          inputRef={register({ validate: isEqual })}
          label={t('SETTINGS.ACCOUNT.REPEATNEWPASSWORD')}
          size="small"
          type="password"
          color="primary"
          name="confirmedPassword"
          error={!!errors.confirmedPassword}
          helperText={errors.confirmedPassword ? t('SETTINGS.ACCOUNT.PASSWORDNOTEQUAL') : ''}
          value={confirmedPassword}
          onChange={(event) => setConfirmedPassword(event.target.value as string)}
        />
      </Grid>
    </SettingsCard>
  )
}

export default UserDetailCard
