import React, {forwardRef, useContext, useImperativeHandle} from 'react';
import {Controller, useForm} from 'react-hook-form';
import {useTranslation} from 'react-i18next';

import {
  Checkbox, FormControlLabel, Grid, MenuItem, Select, SelectChangeEvent, StepProps, Typography,
} from '@mui/material';

import i18n from '../../../i18n';
import {SettingsContext} from '../../../shared/context/settings';
import {WizardContext} from '../../../shared/context/wizard/WizardState';
import {Language} from '../../../shared/models/language';
import {ValidationHandleWithSave} from '../Interfaces';

export const Intro = forwardRef<ValidationHandleWithSave, StepProps>(function Intro(props, ref) {
  const {t} = useTranslation();
  const {setLanguage, setMonitoring} = useContext(WizardContext);
  const {languages} = useContext(SettingsContext);

  const {trigger, getValues, control, formState: {isValid}} = useForm({
    mode: 'onBlur',
    defaultValues: {
      language: 'en-US',
      enableMonitoring: false,
    },
  });

  useImperativeHandle(ref, () => ({
    async validate(): Promise<boolean> {
      await trigger();
      return Promise.resolve(isValid);
    },
    saveChanges(): void {
      if (isValid) {
        const {language, enableMonitoring} = getValues();
        setLanguage(language);
        setMonitoring(enableMonitoring);
      }
    },
  }));

  const handleLanguageChange = (event: SelectChangeEvent<string>) => {
    const lang = event.target.value;
    i18n.changeLanguage(lang);
  };

  const crowdinText = {__html: t('WIZARD.CROWDINHELP')};
  const introText = {__html: t('WIZARD.INTROTEXT')};

  return (
    <Grid container={true} direction="column" spacing={4}>
      <Grid item={true}>
        <Typography variant="h4" color="primary">
          {t('WIZARD.TITLE')}
        </Typography>
        <Typography variant="body1" dangerouslySetInnerHTML={introText} />
      </Grid>
      <Grid item={true} container={true} direction="column" spacing={1}>
        <Grid item={true}>
          <Typography variant="h6" color="primary">
            {t('WIZARD.LANGUAGE')}
          </Typography>
          <Typography>
            {t('WIZARD.SELECTLANGUAGE')}
          </Typography>
        </Grid>
        <Grid item={true}>
          <Controller
            name="language"
            control={control}
            defaultValue={getValues('language')}
            render={({field}) => (
              <Select
                className="max-width"
                variant="outlined"
                sx={{minWidth: 250}}
                {...field}
                onChange={(value) => {
                  field.onChange(value);
                  handleLanguageChange(value);
                }}
              >
                {languages.map((x: Language) => (
                  <MenuItem key={x.code} value={x.code}>
                    {x.name}
                  </MenuItem>
                ))}
              </Select>
            )}
          />
        </Grid>
        <Grid item={true}>
          <Typography
            variant="body1"
            dangerouslySetInnerHTML={crowdinText}
            sx={{
              'fontStyle': 'italic',
              'fontSize': '0.8rem',
              '& a': {
                color:
                (theme) => theme.palette.mode === 'dark' ?
                  theme.palette.secondary.light :
                  theme.palette.secondary.dark,
              },
            }}
          />
        </Grid>
      </Grid>
      <Grid item={true} container={true} direction="column" spacing={1}>
        <Grid item={true}>
          <Typography variant="h6" color="primary">
            {t('WIZARD.EXCEPTIONLOGGING')}
          </Typography>
          <Typography variant="body1">
            {t('SETTINGS.ROLLBAR.EXCEPTIONLOGGING')}
          </Typography>
        </Grid>
        <Grid item={true}>
          <Controller
            name="enableMonitoring"
            control={control}
            defaultValue={getValues('enableMonitoring')}
            render={({field}) =>
              <FormControlLabel
                control={
                  <Checkbox
                    {...field}
                    checked={getValues('enableMonitoring')}
                    disableRipple={true} color="primary" />
                }
                label={t('SETTINGS.ROLLBAR.ENABLEROLLBAR') as string}
              />
            }
          />
        </Grid>
      </Grid>
    </Grid>
  );
});
