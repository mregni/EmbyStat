import {format} from 'date-fns';
import React, {useContext, useState} from 'react';
import {Controller, useForm} from 'react-hook-form';
import {useTranslation} from 'react-i18next';
import timezones from 'timezones-list';

import {Box, MenuItem, Select, Stack, TextField, Typography} from '@mui/material';

import i18n from '../../../i18n';
import {EsSaveCard} from '../../../shared/components/cards';
import {SettingsContext} from '../../../shared/context/settings';
import {useLocale} from '../../../shared/hooks';
import {Language} from '../../../shared/models/language';

type LanguageForm = {
  language: string;
  dateTimeLanguage: string;
  timeZone: string;
}

export function EsLanguageCard() {
  const {userConfig, languages, save} = useContext(SettingsContext);
  const {getLocale} = useLocale();
  const [saving, setSaving] = useState(false);
  const [privateLocale, setPrivateLocale] = useState(getLocale(userConfig.dateTimeLanguage));
  const {t} = useTranslation();

  const {handleSubmit, control} = useForm<LanguageForm>({
    mode: 'all',
    defaultValues: {
      language: userConfig.language,
      dateTimeLanguage: userConfig.dateTimeLanguage,
      timeZone: userConfig.timeZone,
    },
  });

  console.log(userConfig.timeZone);

  const handleLanguageChange = (event: any) => {
    const lang = event.target.value;
    i18n.changeLanguage(lang);
  };

  const handleDateTumeLanguageChange = (event: any) => {
    const lang = event.target.value;
    setPrivateLocale(getLocale(lang));
  };

  const saveForm = async (data: LanguageForm) => {
    userConfig.language = data.language;
    userConfig.dateTimeLanguage = data.dateTimeLanguage;
    userConfig.timeZone = data.timeZone;
    setSaving(true);
    await save(userConfig);
    setSaving(false);
  };

  return (
    <EsSaveCard
      title={t('SETTINGS.LANGUAGE.TITLE')}
      saveForm={saveForm}
      handleSubmit={handleSubmit}
      saving={saving}
    >
      <Controller
        name="language"
        control={control}
        rules={{required: true}}
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
      />
      <Box sx={{pt: 2}}>
        <Controller
          name="dateTimeLanguage"
          control={control}
          rules={{required: true}}
          render={({
            field: {onChange, onBlur, value, ref},
          }) => (
            <TextField
              select
              fullWidth
              label={t('SETTINGS.DATETIMELANGUAGE.LABEL')}
              value={value}
              onChange={(event) => {
                onChange(event);
                handleDateTumeLanguageChange(event);
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
        />
      </Box>
      <Box sx={{pt: 2}}>
        <Controller
          name="timeZone"
          control={control}
          rules={{required: true}}
          render={({
            field: {onChange, onBlur, value, ref},
          }) => (
            <TextField
              select
              fullWidth
              label={t('SETTINGS.TIMEZONE.LABEL')}
              value={value}
              onChange={(event) => {
                onChange(event);
              }}
              onBlur={onBlur}
              inputRef={ref}
            >
              {
                timezones.map((timezone) => <MenuItem
                  key={timezone.tzCode}
                  value={timezone.tzCode}>
                  {`( ${timezone.utc} ) ${timezone.tzCode}`}
                </MenuItem>)
              }
            </TextField>
          )}
        />
      </Box>
      <Box>
        <Typography variant="h6" sx={{mt: 2}}>
          {t('SETTINGS.LANGUAGE.SAMPLES')}
        </Typography>
      </Box>
      <Stack>
        <Typography variant="body1">
          {t('COMMON.TIME')}: {format(new Date('2022-02-23T17:30:00.000Z'), 'p P', {locale: privateLocale})}
        </Typography>
      </Stack>
    </EsSaveCard>
  );
}
