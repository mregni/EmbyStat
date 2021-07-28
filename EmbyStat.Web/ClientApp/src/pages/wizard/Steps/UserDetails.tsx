import React, { forwardRef, useContext, useImperativeHandle } from 'react'
import Grid from '@material-ui/core/Grid';
import Typography from '@material-ui/core/Typography';
import { EsTextInput } from '../../../shared/components/esTextInput';
import { useTranslation } from 'react-i18next';
import { useForm } from 'react-hook-form';

import { ValidationHandleWithSave, StepProps } from '.'
import { WizardContext } from '../Context/WizardState';


export const UserDetails = forwardRef<ValidationHandleWithSave, StepProps>((props, ref) => {
  const { t } = useTranslation();
  const { wizard, setUserDetails } = useContext(WizardContext);

  const { register, trigger, getValues, formState: { errors, isValid } } = useForm({
    mode: "onBlur",
    defaultValues: {
      username: wizard.username,
      password: wizard.password,
    },
  });

  useImperativeHandle(ref, () => ({
    async validate(): Promise<boolean> {
      await trigger();
      return Promise.resolve(isValid);
    },
    saveChanges(): void {
      if (isValid) {
        const { username, password } = getValues();
        setUserDetails(username, password);
      }
    }
  }));

  const usernameRegister = register('username', { required: true });
  const passwordRegister = register('password', { required: true, minLength: 6 });

  return (
    <Grid container direction="column" spacing={7}>
      <Grid item>
        <Typography variant="h4" color="primary">
          {t('SETTINGS.ACCOUNT.TITLE')}
        </Typography>
      </Grid>
      <Grid item>
        <Typography variant="body1">
          {t('WIZARD.USERDETAILTEXT')}
        </Typography>
      </Grid>
      <Grid
        container
        direction="row"
        item
        xs={12}
        spacing={2}
      >
        <Grid item xs={12} md={6}>
          <EsTextInput
            inputRef={usernameRegister}
            label={t("SETTINGS.ACCOUNT.USERNAME")}
            error={errors.username}
            defaultValue={getValues('username')}
            errorText={{
              required: t("SETTINGS.ACCOUNT.NOUSERNAME")
            }}
          />
        </Grid>
        <Grid item xs={12} md={6}>
          <EsTextInput
            inputRef={passwordRegister}
            label={t("SETTINGS.ACCOUNT.PASSWORD")}
            error={errors.password}
            type="password"
            defaultValue={getValues('password')}
            errorText={{
              required: t("SETTINGS.ACCOUNT.NOPASSWORD"),
              minLength: t("SETTINGS.ACCOUNT.PASSWORDMINLENGTH")
            }}
          />
        </Grid>
      </Grid>
    </Grid>
  )
})
