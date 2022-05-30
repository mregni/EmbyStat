import {t} from 'i18next';
import React, {useContext} from 'react';
import {useForm} from 'react-hook-form';

import {Typography} from '@mui/material';

import {EsSaveCard} from '../../../shared/components/cards';
import {EsTextInput} from '../../../shared/components/esTextInput';
import {SettingsContext} from '../../../shared/context/settings';

type TmdbApiForm = {
  apiKey: string;
}

export function EsTmdbApiCard() {
  const {userConfig, save} = useContext(SettingsContext);

  const {handleSubmit, register, getValues, formState: {errors}} = useForm<TmdbApiForm>({
    mode: 'all',
    defaultValues: {
      apiKey: '',
    },
  });

  const saveForm = async (data: TmdbApiForm) => {
    userConfig.tmdb.apiKey = data.apiKey;
    await save(userConfig);
  };

  const apiKeyRegister = register('apiKey', {required: true});

  return (
    <EsSaveCard
      title={t('SETTINGS.TMDB.TITLE')}
      saveForm={saveForm}
      handleSubmit={handleSubmit}
    >
      <Typography variant="body1">
        {t('SETTINGS.TMDB.APIKEYWARNING')}
      </Typography>
      <EsTextInput
        inputRef={apiKeyRegister}
        label={t('SETTINGS.TMDB.LABEL')}
        defaultValue={getValues('apiKey')}
        errorText={{
          required: t('SETTINGS.ACCOUNT.NOUSERNAME'),
        }}
        error={errors.apiKey}
      />
    </EsSaveCard>
  );
}
