import React, { ReactElement, useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import Grid from '@material-ui/core/Grid';
import Select from '@material-ui/core/Select';
import MenuItem from '@material-ui/core/MenuItem';
import Typography from '@material-ui/core/Typography';
import FormControlLabel from '@material-ui/core/FormControlLabel';
import Checkbox from '@material-ui/core/Checkbox';
import { useDispatch, useSelector } from 'react-redux';
import i18next from 'i18next';
import moment from 'moment';

import { loadLanguages } from '../../../../store/LanguageSlice';
import { RootState } from '../../../../store/RootReducer';
import { Language } from '../../../../shared/models/language';
import { setlanguage } from '../../../../store/WizardSlice';

interface Props {
  disableBack: Function;
}

const Intro = (props: Props): ReactElement => {
  const { disableBack } = props;
  const dispatch = useDispatch();
  const { t } = useTranslation();
  const [language, setLanguage] = useState(i18next.language);
  const [rollbar, setRollbar] = useState(false);

  useEffect(() => {
    disableBack(true);
  }, [disableBack]);

  useEffect(() => {
    dispatch(loadLanguages());
    dispatch(setlanguage(i18next.language));
  }, [dispatch]);

  const languages = useSelector((state: RootState) => state.languages);

  const handleChange = (event) => {
    const lang = event.target.value;
    setLanguage(lang);
    i18next.changeLanguage(lang);
    dispatch(setlanguage(lang));
    moment.locale(lang);
  };

  const handelRollbarChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setRollbar(event.target.checked);
  }

  return (
    <Grid container direction="column" spacing={7}>
      <Grid item>
        <Typography variant="h4" color="primary">
          {t('WIZARD.WIZARDLABEL')}
        </Typography>
      </Grid>
      <Grid item container direction="column" spacing={1}>
        <Grid item>
          <Typography variant="body1">
            {t('WIZARD.INTROTEXT')}
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
      </Grid>
      <Grid item container direction="column" spacing={1}>
        <Grid item>
          <Typography variant="body1">
            {t('SETTINGS.ROLLBAR.EXCEPTIONLOGGING')}
          </Typography>
        </Grid>
        <Grid item>
          <FormControlLabel
            control={<Checkbox checked={rollbar} onChange={handelRollbarChange} color="primary" />}
            label={t('SETTINGS.ROLLBAR.ENABLEROLLBAR')}
          />
        </Grid>
      </Grid>
    </Grid>
  );
};

export default Intro;
