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
  const {userConfig, save} = useContext(SettingsContext);
  const {t} = useTranslation();

  const saveForm = async (data: RollbarForm) => {
    userConfig.enableRollbarLogging = data.enabled;
    await save(userConfig);
  };

  const {handleSubmit, control} = useForm<RollbarForm>({
    mode: 'all',
    defaultValues: {
      enabled: userConfig.enableRollbarLogging,
    },
  });

  if (userConfig === null) {
    return (null);
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
        defaultValue={userConfig.enableRollbarLogging}
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
