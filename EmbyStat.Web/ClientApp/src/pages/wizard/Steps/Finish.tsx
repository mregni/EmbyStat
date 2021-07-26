import React, { forwardRef, useContext, useImperativeHandle } from 'react'
import Grid from '@material-ui/core/Grid'
import Typography from '@material-ui/core/Typography';
import { useTranslation } from 'react-i18next';
import { Controller, useForm } from 'react-hook-form';

import { ValidationHandleWithSave } from '.'
import FormControlLabel from '@material-ui/core/FormControlLabel';
import Checkbox from '@material-ui/core/Checkbox';
import { WizardContext } from '../Context/WizardState';

export const Finish = forwardRef<ValidationHandleWithSave>((props, ref) => {
  const { wizard, setFireSyncOnEnd } = useContext(WizardContext);
  const { t } = useTranslation();

  const { getValues, control } = useForm({
    mode: "onBlur",
    defaultValues: {
      fireSync: false
    }
  });

  useImperativeHandle(ref, () => ({
    async validate(): Promise<boolean> {
      return Promise.resolve(true);
    },
    saveChanges(): void {
      const { fireSync } = getValues();
      setFireSyncOnEnd(fireSync);
    }
  }));

  return (
    <Grid container direction="column">
      <Typography variant="h4" color="primary">
        {t('WIZARD.FINALLABEL')}
      </Typography>
      <Grid container direction="column">
        <Typography variant="body1" className="m-t-16 m-b-16">
          {t('WIZARD.FINISHED', { type: wizard.serverType })}
        </Typography>
        <Typography variant="body1">
          {t('WIZARD.FINISHEXPLANATION', { type: wizard.serverType })}
        </Typography>
        <Grid item>
          <Controller
            name="fireSync"
            control={control}
            defaultValue={getValues('fireSync')}
            render={({ field }) => <FormControlLabel control={
              <Checkbox {...field} checked={getValues('fireSync')} disableRipple color="primary" />
            }
              label={t("WIZARD.RUNSYNC")}
            />}
          />
        </Grid>
      </Grid>
    </Grid>
  )
})
