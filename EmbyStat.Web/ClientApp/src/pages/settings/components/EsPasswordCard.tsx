import React, {useContext} from 'react';
import {useForm} from 'react-hook-form';
import {useTranslation} from 'react-i18next';

import {Typography} from '@mui/material';

import {EsSaveCard} from '../../../shared/components/cards';
import {EsTextInput} from '../../../shared/components/esTextInput';
import {UserContext} from '../../../shared/context/user';
import SnackbarUtils from '../../../shared/utils/SnackbarUtilsConfigurator';

type Password = {
  password: string;
  confirmedPassword: string;
  currentPassword: string;
}

export function EsPasswordCard() {
  const {changePassword, user, login} = useContext(UserContext);
  const {t} = useTranslation();

  const submitForm = async (data: Password) => {
    if (user !== null) {
      const result = await changePassword(data.password, data.currentPassword);

      if (result) {
        await login(user.username, data.password);
        SnackbarUtils.success(t('SETTINGS.PASSWORD.PASSWORDUPDATED'));
        return;
      }

      SnackbarUtils.error(t('SETTINGS.PASSWORD.PASSWORDUPDATEFAILED'));
    }
  };

  const {register, handleSubmit, getValues, formState: {errors}} = useForm<Password>({
    mode: 'all',
    defaultValues: {
      password: '',
      confirmedPassword: '',
      currentPassword: '',
    },
  });

  const isEqual = (value: string) => value === getValues('password');

  const passwordRegister = register('password', {required: true, minLength: 6});
  const confirmPasswordRegister = register('confirmedPassword', {validate: isEqual});
  const currentPasswordRegister = register('currentPassword', {required: true});

  return (
    <EsSaveCard
      title='SETTINGS.PASSWORD.TITLE'
      saveForm={submitForm}
      handleSubmit={handleSubmit}
    >
      <Typography variant="body1">
        {t('SETTINGS.PASSWORD.CONTENT')}
      </Typography>
      <EsTextInput
        inputRef={currentPasswordRegister}
        defaultValue={getValues('currentPassword')}
        label={t('SETTINGS.PASSWORD.CURRENTPASSWORD')}
        type="password"
        errorText={{
          required: t('SETTINGS.PASSWORD.NOCURRENTPASSWORD'),
        }}
        error={errors.currentPassword}
      />
      <EsTextInput
        inputRef={passwordRegister}
        defaultValue={getValues('password')}
        label={t('SETTINGS.PASSWORD.NEWPASSWORD')}
        errorText={{
          required: t('SETTINGS.PASSWORD.NOPASSWORD'),
          minLength: t('SETTINGS.PASSWORD.PASSWORDMINLENGTH'),
        }}
        type="password"
        error={errors.currentPassword}
      />
      <EsTextInput
        inputRef={confirmPasswordRegister}
        defaultValue={getValues('confirmedPassword')}
        label={t('SETTINGS.PASSWORD.REPEATNEWPASSWORD')}
        errorText={{
          validate: t('SETTINGS.PASSWORD.PASSWORDNOTEQUAL'),
        }}
        type="password"
        error={errors.confirmedPassword}
      />
    </EsSaveCard>
  );
}
