import React, { useState } from 'react';
import Grid from '@material-ui/core/Grid';
import Typography from '@material-ui/core/Typography';
import FormControlLabel from '@material-ui/core/FormControlLabel';
import Checkbox from '@material-ui/core/Checkbox';
import { useSelector, useDispatch } from 'react-redux';
import { useTranslation } from 'react-i18next';

import { RootState } from '../../../store/RootReducer';
import { saveSettings } from '../../../store/SettingsSlice';
import SnackbarUtils from '../../../shared/utils/SnackbarUtilsConfigurator';
import SettingsCard from '../SettingsCard';

interface Props {
  delay: number
}

const RollbarCard = (props: Props) => {
  const { delay } = props;
  const settings = useSelector((state: RootState) => state.settings);
  const [rollbar, setRollbar] = useState(settings.enableRollbarLogging);
  const { t } = useTranslation();
  const dispatch = useDispatch();

  const saveForm = () => {
    const newSettings = { ...settings };
    newSettings.enableRollbarLogging = rollbar;
    dispatch(saveSettings(newSettings));
    SnackbarUtils.info(t('SETTINGS.ROLLBAR.SAVING'));
  }

  const handleChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setRollbar(event.target.checked);
  }

  return (
    <SettingsCard
      delay={delay}
      title={t('SETTINGS.ROLLBAR.TITLE')}
      saveForm={saveForm}
    >
      <Grid item className="m-t-16">
        <Typography variant="body2">
          {t('SETTINGS.ROLLBAR.EXCEPTIONLOGGING')}
        </Typography>
      </Grid>
      <Grid item>
        <FormControlLabel
          control={<Checkbox checked={rollbar} onChange={handleChange} color="primary" />}
          label={t('SETTINGS.ROLLBAR.ENABLEROLLBAR')}
        />
      </Grid>
    </SettingsCard>
  )
}

export default RollbarCard
