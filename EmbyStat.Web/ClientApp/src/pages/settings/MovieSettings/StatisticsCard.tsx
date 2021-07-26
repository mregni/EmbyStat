import React, { useContext, useState } from 'react'
import { useTranslation } from 'react-i18next';
import Grid from '@material-ui/core/Grid';
import Typography from '@material-ui/core/Typography';
import { useDispatch } from 'react-redux';
import { useForm } from 'react-hook-form';
import FormControlLabel from '@material-ui/core/FormControlLabel';
import Checkbox from '@material-ui/core/Checkbox';

import SettingsCard from '../SettingsCard';
import { saveSettings } from '../../../store/SettingsSlice';
import SnackbarUtils from '../../../shared/utils/SnackbarUtilsConfigurator';
import { EsTextInput } from '../../../shared/components/esTextInput';
import { SettingsContext } from '../../../shared/context/settings';

interface Props {
  delay: number;
}

export const StatisticsCard = (props: Props) => {
  const { delay } = props;
  const { t } = useTranslation();
  const dispatch = useDispatch();
  const { settings } = useContext(SettingsContext);
  const [value, setValue] = useState(settings.toShortMovie);
  const [enabled, setEnabled] = useState(settings.toShortMovieEnabled)

  const saveForm = async () => {
    var result = await trigger();
    if (result) {
      const newSettings = { ...settings };
      newSettings.toShortMovie = value;
      newSettings.toShortMovieEnabled = enabled;
      dispatch(saveSettings(newSettings));
      SnackbarUtils.info(t('SETTINGS.STATISTICS.SAVING', { type: t('COMMON.MOVIE') }));
    }
  }

  const { register, trigger, getValues, formState: { errors } } = useForm({
    mode: 'onBlur',
    defaultValues: {
      toShortEnabled: settings.toShortMovieEnabled,
      toShortValue: settings.toShortMovie,
    }
  });

  const handleChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setEnabled(event.target.checked);
    if (event.target.checked === false) {
      setValue(0);
    }
  };

  const toShortValueRegister = register('toShortValue');

  return (
    <SettingsCard
      delay={delay}
      title={t('COMMON.STATISTICS')}
      saveForm={saveForm}
    >
      <Grid item className="m-t-16">
        <Typography variant="body2">
          {t('SETTINGS.STATISTICS.TOSHORTTEXT')}
        </Typography>
      </Grid>
      <Grid item>
        <FormControlLabel
          control={<Checkbox checked={enabled} onChange={handleChange} color="primary" />}
          label={t('SETTINGS.STATISTICS.ENABLETOSHORTSCAN')}
        />
      </Grid>
      <Grid item>
        <EsTextInput
          inputRef={toShortValueRegister}
          defaultValue={getValues('toShortValue')}
          label={t('SETTINGS.STATISTICS.TOSHORTMINUTES')}
          error={errors.toShortValue}
          errorText={{ required: t('FORMERRORS.REQUIRED') }}
          readonly={!enabled}
          type="number"
          onChange={(value: string) => setValue(parseInt(value, 10))}
        />
      </Grid>
    </SettingsCard>
  )
}
