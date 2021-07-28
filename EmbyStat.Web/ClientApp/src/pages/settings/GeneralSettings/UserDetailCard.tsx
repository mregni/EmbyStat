import React, { useState, useEffect } from 'react';
import Grid from '@material-ui/core/Grid';
import Typography from '@material-ui/core/Typography';
import { makeStyles } from '@material-ui/core/styles';
import { useForm } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import classNames from 'classnames';

import { getUserInfo, changePassword, changeUserName, login } from '../../../shared/services/AccountService';
import SnackbarUtils from '../../../shared/utils/SnackbarUtilsConfigurator';
import { User } from '../../../shared/models/login';
import SettingsCard from '../SettingsCard';
import { EsTextInput } from '../../../shared/components/esTextInput';

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

export const UserDetailCard = (props: Props) => {
  const { delay } = props;
  const classes = useStyles();
  const [user, setUser] = useState<User | null>(null);
  const { t } = useTranslation();

  useEffect(() => {
    const name = getUserInfo();
    setUser(name);
    if (name !== null) {
      setValue('username', name.username);
    }
  }, []);

  const { register, trigger, getValues, setValue, formState: { errors } } = useForm({
    mode: 'onBlur',
    defaultValues: {
      username: user?.username ?? '',
      password: '',
      confirmedPassword: '',
      currentPassword: '',
    }
  });

  const saveUser = async () => {
    await updateUserName();
    await updatePassword();
  }

  const updateUserName = async () => {
    const { username, currentPassword } = getValues();
    if (username !== user?.username) {
      const valid = await trigger(['username', 'currentPassword']);
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
    const { password, currentPassword, username } = getValues();
    if (password.length !== 0) {
      const valid = await trigger(['confirmedPassword', 'password', 'currentPassword']);
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

  const isEqual = (value: string) => value === getValues('password');

  const usernameRegister = register('username', { required: true });
  const passwordRegister = register('password', { required: true, minLength: 6 });
  const confirmPasswordRegister = register('confirmedPassword', { validate: isEqual });
  const currentPasswordRegister = register('currentPassword', { required: true });

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
        <EsTextInput
          inputRef={usernameRegister}
          defaultValue={getValues('username')}
          label={t('SETTINGS.ACCOUNT.USERNAME')}
          errorText={{
            required: t('SETTINGS.ACCOUNT.NOUSERNAME')
          }}
          error={errors.username}
        />
      </Grid>
      <Grid item className={classNames({
        [classes.normall__input]: !errors.currentPassword,
        [classes.error__input]: errors.currentPassword,
      })}>
        <EsTextInput
          inputRef={currentPasswordRegister}
          defaultValue={getValues('currentPassword')}
          label={t('SETTINGS.ACCOUNT.CURRENTPASSWORD')}
          errorText={{
            required: t('SETTINGS.ACCOUNT.NOPASSWORD')
          }}
          error={errors.currentPassword}
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
        <EsTextInput
          inputRef={passwordRegister}
          defaultValue={getValues('password')}
          label={t('SETTINGS.ACCOUNT.NEWPASSWORD')}
          errorText={{
            required: t('SETTINGS.ACCOUNT.NOPASSWORD'),
            minLength: t('SETTINGS.ACCOUNT.PASSWORDMINLENGTH')
          }}
          type="password"
          error={errors.currentPassword}
        />
      </Grid>
      <Grid item className={classNames({
        [classes.normall__input]: !errors.confirmedPassword,
        [classes.error__input]: errors.confirmedPassword
      })}>
        <EsTextInput
          inputRef={confirmPasswordRegister}
          defaultValue={getValues('confirmedPassword')}
          label={t('SETTINGS.ACCOUNT.REPEATNEWPASSWORD')}
          errorText={{
            validate: t('SETTINGS.ACCOUNT.PASSWORDNOTEQUAL')
          }}
          type="password"
          error={errors.confirmedPassword}
        />
      </Grid>
    </SettingsCard>
  )
}
