import React, { forwardRef, useContext, useEffect, useImperativeHandle, useState } from 'react';
import { useTranslation } from "react-i18next";
import { makeStyles } from "@material-ui/core/styles";
import { Controller, useForm } from 'react-hook-form';
import moment from "moment";
import Grid from '@material-ui/core/Grid';
import Typography from '@material-ui/core/Typography';
import Select from '@material-ui/core/Select';
import MenuItem from '@material-ui/core/MenuItem';
import FormControlLabel from '@material-ui/core/FormControlLabel';
import Checkbox from '@material-ui/core/Checkbox';
import i18n from '../../../i18n';

import { StepProps, ValidationHandleWithSave } from '.';
import { WizardContext } from '../Context/WizardState';
import { Language } from '../../../shared/models/language';
import { FetchState, useLanguages } from '../../../shared/hooks';

const useStyles = makeStyles((theme) => ({
  link: {
    fontStyle: "italic",
    fontSize: "0.8rem",
    "& a": {
      color:
        theme.palette.type === "dark"
          ? theme.palette.secondary.light
          : theme.palette.secondary.dark,
    },
  },
}));

export const Intro = forwardRef<ValidationHandleWithSave, StepProps>((props, ref) => {
  const classes = useStyles();
  const { t } = useTranslation();
  const { setLanguage, setMonitoring, wizard } = useContext(WizardContext);
  const { languageList, state } = useLanguages();

  const { trigger, getValues, control, formState: { isValid } } = useForm({
    mode: "onBlur",
    defaultValues: {
      language: wizard.language ?? 'en-US',
      enableMonitoring: wizard.enableRollbarLogging
    }
  });

  useImperativeHandle(ref, () => ({
    async validate(): Promise<boolean> {
      await trigger();
      return Promise.resolve(isValid);
    },
    saveChanges(): void {
      if (isValid) {
        const { language, enableMonitoring } = getValues();
        setLanguage(language);
        setMonitoring(enableMonitoring);
      }
    }
  }));

  const handleLanguageChange = (event) => {
    const lang = event.target.value;
    i18n.changeLanguage(lang);
    moment.locale(lang);
  };

  const crowdinText = { __html: t("WIZARD.CROWDINHELP") };
  const introText = { __html: t("WIZARD.INTROTEXT") };

  return (
    <Grid container direction="column" spacing={7}>
      <Grid item>
        <Typography variant="h4" color="primary">
          {t("WIZARD.WIZARDLABEL")}
        </Typography>
      </Grid>
      <Grid item container direction="column" spacing={1}>
        <Grid item>
          <Typography variant="body1" dangerouslySetInnerHTML={introText} />
        </Grid>
        <Grid item>
          <Controller
            name="language"
            control={control}
            defaultValue={getValues('language')}
            render={({ field }) => (
              <Select
                className="max-width"
                variant="standard"
                {...field}
                onChange={(value) => {
                  field.onChange(value);
                  handleLanguageChange(value);
                }}
              >
                {state === FetchState.success && languageList.map((x: Language) => (
                  <MenuItem key={x.code} value={x.code}>
                    {x.name}
                  </MenuItem>
                ))}
              </Select>
            )}
          />
        </Grid>
        <Grid item>
          <Typography
            variant="body1"
            dangerouslySetInnerHTML={crowdinText}
            className={classes.link}
          />
        </Grid>
      </Grid>
      <Grid item container direction="column" spacing={1}>
        <Grid item>
          <Typography variant="body1">
            {t("SETTINGS.ROLLBAR.EXCEPTIONLOGGING")}
          </Typography>
        </Grid>
        <Grid item>
          <Controller
            name="enableMonitoring"
            control={control}
            defaultValue={getValues('enableMonitoring')}
            render={({ field }) => <FormControlLabel control={<Checkbox {...field} checked={getValues('enableMonitoring')} disableRipple color="primary" />} label={t("SETTINGS.ROLLBAR.ENABLEROLLBAR")} />}
          />
        </Grid>
      </Grid>
    </Grid>
  );
})
