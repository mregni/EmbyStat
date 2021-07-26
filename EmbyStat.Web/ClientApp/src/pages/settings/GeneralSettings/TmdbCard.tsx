import React, { useContext, useState } from 'react';
import Grid from '@material-ui/core/Grid';
import Typography from '@material-ui/core/Typography';
import { useSelector, useDispatch } from 'react-redux';
import { useTranslation } from 'react-i18next';
import { useForm } from 'react-hook-form';
import classNames from 'classnames';
import { makeStyles } from '@material-ui/core/styles';

import { RootState } from '../../../store/RootReducer';
import { saveSettings } from '../../../store/SettingsSlice';
import SnackbarUtils from '../../../shared/utils/SnackbarUtilsConfigurator';
import SettingsCard from '../SettingsCard';
import { EsTextInput } from '../../../shared/components/esTextInput';
import { SettingsContext } from '../../../shared/context/settings';

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

export const TmdbCard = (props: Props) => {
  const { delay } = props;
  const classes = useStyles();
  const { settings } = useContext(SettingsContext);
  const [key, setKey] = useState(settings.tmdb.apiKey);
  const { t } = useTranslation();
  const dispatch = useDispatch();

  const saveForm = async () => {
    var result = await trigger();
    if (result) {
      const newSettings = { ...settings };
      const tmdbSetting = { ...newSettings.tmdb };
      tmdbSetting.apiKey = key;
      newSettings.tmdb = tmdbSetting;
      dispatch(saveSettings(newSettings));
      SnackbarUtils.info(t('SETTINGS.TMDB.SAVING'));
    }
  }

  const { register, trigger, getValues, formState: { errors } } = useForm({
    mode: 'onBlur',
    defaultValues: {
      key: ''
    }
  });

  const keyRegister = register('key', { required: true });

  return (
    <SettingsCard
      delay={delay}
      title={t('SETTINGS.TMDB.TITLE')}
      saveForm={saveForm}
    >
      <Grid item className="m-t-16">
        <Typography variant="body2">
          {t('SETTINGS.TMDB.APIKEYWARNING')}
        </Typography>
      </Grid>
      <Grid item className={classNames({
        [classes.normall__input]: !errors.key,
        [classes.error__input]: errors.key,
      })}>
        <EsTextInput
          inputRef={keyRegister}
          errorText={{
            required: t('SETTINGS.TMDB.NOAPIKEY')
          }}
          label={t('SETTINGS.TMDB.APIKEYLABEL')}
          error={errors.key}
          onChange={(value: string) => setKey(value)}
          defaultValue={getValues('key')}
        />
      </Grid>
    </SettingsCard>
  )
}
