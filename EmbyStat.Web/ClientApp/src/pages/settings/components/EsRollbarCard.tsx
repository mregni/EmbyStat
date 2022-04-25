import React, {useContext} from 'react';
import {Controller, useForm} from 'react-hook-form';
import {useTranslation} from 'react-i18next';

import {Checkbox, FormControlLabel, Typography} from '@mui/material';

import {EsSaveCard} from '../../../shared/components/cards';
import {SettingsContext} from '../../../shared/context/settings';

type RollbarForm = {
  enabled: boolean;
}

export function EsRollbarCard() {
  const {settings, save} = useContext(SettingsContext);
  const {t} = useTranslation();

  const saveForm = async (data: RollbarForm) => {
    settings.enableRollbarLogging = data.enabled;
    await save(settings);
  };

  const {handleSubmit, control} = useForm<RollbarForm>({
    mode: 'all',
    defaultValues: {
      enabled: settings.enableRollbarLogging,
    },
  });

  if (settings === null) {
    return <></>;
  }

  return (
    <EsSaveCard
      title='SETTINGS.ROLLBAR.TITLE'
      saveForm={saveForm}
      handleSubmit={handleSubmit}
    >
      <Typography variant="body1">
        {t('SETTINGS.ROLLBAR.EXCEPTIONLOGGING')}
      </Typography>
      <Controller
        name="enabled"
        control={control}
        defaultValue={settings.enableRollbarLogging}
        render={({field: {onChange, onBlur, value, ref}}) =>
          <FormControlLabel
            control={
              <Checkbox
                checked={value}
                onBlur={onBlur}
                onChange={onChange}
                inputRef={ref}
                color="primary" />}
            label={t('SETTINGS.ROLLBAR.ENABLEROLLBAR') as string}
          />
        }
      />
    </EsSaveCard>
  );
}
