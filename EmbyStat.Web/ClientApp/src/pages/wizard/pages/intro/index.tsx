import React, { ReactElement, useEffect } from 'react';
import { Trans } from 'react-i18next';

import { Grid, Typography } from '@material-ui/core';
import { useDispatch, useSelector } from 'react-redux';
import i18next from 'i18next';
import { loadLanguages } from '../../../../store/LanguageSlice';
import { RootState } from '../../../../store/RootReducer';
import EmbyStatSelect from '../../../../shared/components/inputs/select/EmbyStatSelect';
import { Language } from '../../../../shared/models/language';

import { setlanguage } from '../../../../store/WizardSlice';
import moment from 'moment';

interface Props {
  disableBack: Function,
}

const Intro = (props: Props): ReactElement => {
  const { disableBack } = props;
  const dispatch = useDispatch();
  const [language, setLanguage] = React.useState(i18next.language);

  useEffect(() => {
    disableBack(true);
  }, [disableBack]);

  useEffect(() => {
    dispatch(setlanguage(i18next.language));
  }, [dispatch]);

  useEffect(() => {
    dispatch(loadLanguages());
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
      <Typography variant="h4" color="secondary">
        <Trans i18nKey="WIZARD.WIZARDLABEL" />
      </Typography>
      <Typography variant="body1" className="m-t-32">
        <Trans i18nKey="WIZARD.INTROTEXT" />
      </Typography>
      <Grid container item xs={12} className="m-t-32">
        {languages !== undefined && languages.isLoaded ?
          <EmbyStatSelect
            value={language}
            variant="standard"
            onChange={handleChange}
            menuItems={languages.languages.map((x: Language) => { return { id: x.code, value: x.code, label: x.name } })}
          /> : null}
      </Grid>
    </Grid>
  );
};

export default Intro;
