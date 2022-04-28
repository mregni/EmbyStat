import React, {useContext} from 'react';
import {useForm} from 'react-hook-form';
import {useTranslation} from 'react-i18next';

import {Typography} from '@mui/material';

import {EsSaveCard} from '../../../shared/components/cards';
import {EsTextInput} from '../../../shared/components/esTextInput';
import {UserContext} from '../../../shared/context/user';
import SnackbarUtils from '../../../shared/utils/SnackbarUtilsConfigurator';

type User = {
  username: string;
}

export function EsUserCard() {
  const {user, changeUserName} = useContext(UserContext);
  const {t} = useTranslation();

  const {register, handleSubmit, getValues, formState: {errors}} = useForm<User>({
    mode: 'all',
    defaultValues: {
      username: user?.username ?? '',
    },
  });

  const submitForm = async (data: User) => {
    if (user !== null) {
      const result = await changeUserName(data.username);
      if (result) {
        SnackbarUtils.success(t('SETTINGS.ACCOUNT.USERNAMEUPDATED'));
        return;
      }
      SnackbarUtils.error(t('SETTINGS.ACCOUNT.USERNAMEUPDATEFAILED'));
    }
  };

  const usernameRegister = register('username', {required: true});

  return (
    <EsSaveCard
      title='SETTINGS.ACCOUNT.USERNAME'
      saveForm={submitForm}
      handleSubmit={handleSubmit}
    >
      <Typography variant="body1">
        {t('SETTINGS.ACCOUNT.CONTENT')}
      </Typography>
      <EsTextInput
        inputRef={usernameRegister}
        label={t('SETTINGS.ACCOUNT.USERNAME')}
        defaultValue={getValues('username')}
        errorText={{
          required: t('SETTINGS.ACCOUNT.NOUSERNAME'),
        }}
        error={errors.username}
      />
    </EsSaveCard>
  );
}
