import React, { ReactElement, useEffect } from 'react';
import { Trans } from 'react-i18next';
import Grid from '@material-ui/core/Grid';
import Select from '@material-ui/core/Select';
import MenuItem from '@material-ui/core/MenuItem';
import Typography from '@material-ui/core/Typography';
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
  const [language, setLanguage] = React.useState(i18next.language);

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

  return (
    <Grid container direction="column">
      <Typography variant="h4" color="primary">
        <Trans i18nKey="WIZARD.WIZARDLABEL" />
      </Typography>
      <Typography variant="body1" className="m-t-32">
        <Trans i18nKey="WIZARD.INTROTEXT" />
      </Typography>
      <Grid container item xs={12} className="m-t-32">
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
  );
};

export default Intro;
