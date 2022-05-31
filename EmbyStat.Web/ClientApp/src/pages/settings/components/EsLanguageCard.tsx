import {format} from 'date-fns';
import React, {useContext, useState} from 'react';
import {Controller, useForm} from 'react-hook-form';
import {useTranslation} from 'react-i18next';

import {Box, MenuItem, Stack, TextField, Typography} from '@mui/material';

import i18n from '../../../i18n';
import {EsSaveCard} from '../../../shared/components/cards';
import {SettingsContext} from '../../../shared/context/settings';
import {useLocale} from '../../../shared/hooks';
import {Language} from '../../../shared/models/language';

type LanguageForm = {
  language: string;
}

export function EsLanguageCard() {
  const {userConfig, languages, save} = useContext(SettingsContext);
  const {getLocale} = useLocale();
  const [privateLocale, setPrivateLocale] = useState(getLocale(userConfig.language));
  const {t} = useTranslation();

  const {handleSubmit, control} = useForm<LanguageForm>({
    mode: 'all',
    defaultValues: {
      language: userConfig.language,
    },
  });

  const handleLanguageChange = (event: any) => {
    const lang = event.target.value;
    setPrivateLocale(getLocale(lang));
    i18n.changeLanguage(lang);
  };

  const saveForm = async (data: LanguageForm) => {
    userConfig.language = data.language;
    await save(userConfig);
  };

  return (
    <EsSaveCard
      title={t('SETTINGS.LANGUAGE.TITLE')}
      saveForm={saveForm}
      handleSubmit={handleSubmit}
    >
      <Controller
        render={({
          field: {onChange, onBlur, value, ref},
        }) => (
          <TextField
            select
            fullWidth
            label={t('SETTINGS.LANGUAGE.LABEL')}
            value={value}
            onChange={(event) => {
              onChange(event);
              handleLanguageChange(event);
            }}
            onBlur={onBlur}
            inputRef={ref}
          >
            {languages.map((x: Language) => (
              <MenuItem key={x.code} value={x.code}>
                {x.name}
              </MenuItem>
            ))}
          </TextField>
        )}
        name="language"
        control={control}
        rules={{required: true}}
      />
      <Box>
        <Typography variant="h6" sx={{mt: 2}}>
          {t('SETTINGS.LANGUAGE.SAMPLES')}
        </Typography>
      </Box>
      <Stack>
        <Typography variant="body1">
          {t('COMMON.TIME')}: {format(new Date('2022-02-23T17:30:00.000Z'), 'p', {locale: privateLocale})}
        </Typography>
        <Typography variant="body1">
          {t('COMMON.DATE')}: {format(new Date('2022-02-23T17:30:00.000Z'), 'P', {locale: privateLocale})}
        </Typography>
      </Stack>
    </EsSaveCard>
  );
}
