import React, { useState } from 'react';
import Grid from '@material-ui/core/Grid';
import Typography from '@material-ui/core/Typography';
import TextField from '@material-ui/core/TextField';
import { useSelector, useDispatch } from 'react-redux';
import { useTranslation } from 'react-i18next';
import { useForm } from 'react-hook-form';
import classNames from 'classnames';
import { makeStyles } from '@material-ui/core/styles';

import { RootState } from '../../../store/RootReducer';
import { saveSettings } from '../../../store/SettingsSlice';
import SnackbarUtils from '../../../shared/utils/SnackbarUtilsConfigurator';
import SettingsCard from '../SettingsCard';

const useStyles = makeStyles({
  normall__input: {
    paddingBottom: 25,
  },
  error__input: {
    paddingBottom: 2,
  }
});

interface Props {
  delay: number
}

const TvdbCard = (props: Props) => {
  const { delay } = props;
  const classes = useStyles();
  const settings = useSelector((state: RootState) => state.settings);
  const [key, setKey] = useState(settings.tvdb.apiKey);
  const { t } = useTranslation();
  const dispatch = useDispatch();

  const saveForm = async () => {
    var result = await trigger();
    if (result) {
      const newSettings = { ...settings };
      const tvdbSetting = { ...newSettings.tvdb };
      tvdbSetting.apiKey = key;
      newSettings.tvdb = tvdbSetting;
      dispatch(saveSettings(newSettings));
      SnackbarUtils.info(t('SETTINGS.TVDB.SAVING'));
    }
  }

  const { register, errors, trigger } = useForm({
    mode: 'onBlur',
    defaultValues: {
      key,
    }
  });

  return (
    <SettingsCard
      delay={delay}
      title={t('SETTINGS.TVDB.TITLE')}
      saveForm={saveForm}
    >
      <Grid item className="m-t-16">
        <Typography variant="body2">
          {t('SETTINGS.TVDB.APIKEYWARNING')}
        </Typography>
      </Grid>
      <Grid item className={classNames({
        [classes.normall__input]: !errors.key,
        [classes.error__input]: errors.key,
      })}>
        <TextField
          inputRef={register({ required: t('SETTINGS.TVDB.NOAPIKEY').toString() })}
          label={t('SETTINGS.TVDB.APIKEYLABEL')}
          size="small"
          name="key"
          error={!!errors.key}
          helperText={errors.key ? errors.key.message : ''}
          color="primary"
          value={key}
          onChange={(event) => setKey(event.target.value as string)}
        />
      </Grid>
    </SettingsCard>
  )
}

export default TvdbCard
