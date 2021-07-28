import React, { useState, useEffect, useContext } from 'react';
import Grid from '@material-ui/core/Grid';
import Typography from '@material-ui/core/Typography';
import Select from '@material-ui/core/Select';
import MenuItem from '@material-ui/core/MenuItem';
import { useSelector, useDispatch } from 'react-redux';
import { useTranslation } from 'react-i18next';
import i18next from 'i18next';
import moment from 'moment';

import { RootState } from '../../../store/RootReducer';
import { loadLanguages } from '../../../store/LanguageSlice';
import { Language } from '../../../shared/models/language';
import { saveSettings } from '../../../store/SettingsSlice';
import SnackbarUtils from '../../../shared/utils/SnackbarUtilsConfigurator';
import SettingsCard from '../SettingsCard';
import { SettingsContext } from '../../../shared/context/settings';

interface Props {
  delay: number
}

export const LanguageCard = (props: Props) => {
  const { delay } = props;
  const { settings } = useContext(SettingsContext);
  const [language, setLanguage] = useState(i18next.language);
  const languages = useSelector((state: RootState) => state.languages);
  const { t } = useTranslation();
  const dispatch = useDispatch();

  useEffect(() => {
    dispatch(loadLanguages());
  }, [dispatch]);

  const saveForm = () => {
    i18next.changeLanguage(language);
    moment.locale(language);
    const newSettings = { ...settings };
    newSettings.language = language;
    dispatch(saveSettings(newSettings));
    const languageName = languages.languages.filter((x: Language) => x.code === language)[0].name
    SnackbarUtils.info(t('SETTINGS.LANGUAGE.SAVING', { lang: languageName }));
  }

  const handleChange = (event) => {
    setLanguage(event.target.value);
  };

  return (
    <SettingsCard
      delay={delay}
      title={t('SETTINGS.LANGUAGE.TITLE')}
      saveForm={saveForm}
    >
      <Grid item className="m-t-16">
        <Typography variant="body2">
          {t('SETTINGS.LANGUAGE.LABEL')}
        </Typography>
      </Grid>
      <Grid item>
        {languages !== undefined && languages.isLoaded ? (
          <Select
            className="max-width"
            variant="standard"
            onChange={handleChange}
            value={language}
            name='language'
          >
            {languages.languages.map((x: Language) => (
              <MenuItem key={x.code} value={x.code}>
                {x.name}
              </MenuItem>
            ))}
          </Select>
        ) : null}
      </Grid>
    </SettingsCard>
  )
}
