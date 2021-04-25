import React, { useState } from 'react'
import { useTranslation } from 'react-i18next';
import Grid from '@material-ui/core/Grid';
import Typography from '@material-ui/core/Typography';
import TextField from '@material-ui/core/TextField';
import { useSelector, useDispatch } from 'react-redux';
import { useForm } from 'react-hook-form';
import FormControlLabel from '@material-ui/core/FormControlLabel';
import Checkbox from '@material-ui/core/Checkbox';

import SettingsCard from '../SettingsCard';
import { RootState } from '../../../store/RootReducer';
import { saveSettings } from '../../../store/SettingsSlice';
import SnackbarUtils from '../../../shared/utils/SnackbarUtilsConfigurator';

interface Props {
  delay: number;
}

const StatisticsCard = (props: Props) => {
  const { delay } = props;
  const { t } = useTranslation();
  const dispatch = useDispatch();
  const settings = useSelector((state: RootState) => state.settings);
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

  const { register, errors, trigger } = useForm({
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
        <TextField
          inputRef={register({ required: t('FORMERRORS.REQUIRED').toString() })}
          label={t('SETTINGS.STATISTICS.TOSHORTMINUTES')}
          size="small"
          name="key"
          type="number"
          error={!!errors.toShortValue}
          helperText={errors.toShortValue ? errors.toShortValue.message : ''}
          color="primary"
          value={value}
          disabled={!enabled}
          onChange={(event) => setValue(parseInt(event.target.value as string, 10))}
        />
      </Grid>
    </SettingsCard>
  )
}

export default StatisticsCard
