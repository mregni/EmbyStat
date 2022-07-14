import {t} from 'i18next';
import React, {useContext, useState} from 'react';
import {Controller, useForm} from 'react-hook-form';

import {Checkbox, FormControlLabel, Typography} from '@mui/material';

import {EsSaveCard} from '../../../shared/components/cards';
import {EsTextInput} from '../../../shared/components/esTextInput';
import {SettingsContext} from '../../../shared/context/settings';

type TmdbApiForm = {
  apiKey: string;
  enableFallback: boolean;
}

export function EsTmdbApiCard() {
  const {userConfig, save} = useContext(SettingsContext);
  const [saving, setSaving] = useState(false);

  const {handleSubmit, register, getValues, formState: {errors}, control} = useForm<TmdbApiForm>({
    mode: 'all',
    defaultValues: {
      apiKey: userConfig.tmdb.apiKey,
      enableFallback: userConfig.tmdb.useAsFallback,
    },
  });

  const saveForm = async (data: TmdbApiForm) => {
    userConfig.tmdb.apiKey = data.apiKey;
    userConfig.tmdb.useAsFallback = data.enableFallback;

    setSaving(true);
    await save(userConfig);
    setSaving(false);
  };

  const apiKeyRegister = register('apiKey', {required: true});

  return (
    <EsSaveCard
      title={t('SETTINGS.TMDB.TITLE')}
      saveForm={saveForm}
      handleSubmit={handleSubmit}
      saving={saving}
    >
      <Typography variant="body1">
        {t('SETTINGS.TMDB.APIKEYWARNING')}
      </Typography>
      <EsTextInput
        inputRef={apiKeyRegister}
        label={t('SETTINGS.TMDB.LABEL')}
        defaultValue={getValues('apiKey')}
        errorText={{
          required: t('SETTINGS.TMDB.NOAPIKEY'),
        }}
        error={errors.apiKey}
      />
      <Controller
        name="enableFallback"
        control={control}
        defaultValue={userConfig.tmdb.useAsFallback}
        render={({field: {onChange, onBlur, value, ref}}) =>
          <FormControlLabel
            control={
              <Checkbox
                checked={value}
                onBlur={onBlur}
                onChange={onChange}
                inputRef={ref}
                color="primary" />}
            label={t('SETTINGS.TMDB.FALLBACK') as string}
          />
        }
      />
    </EsSaveCard>
  );
}
