import React, {forwardRef, useContext, useImperativeHandle} from 'react';
import {useForm} from 'react-hook-form';
import {useTranslation} from 'react-i18next';

import {Box, Stack, Typography} from '@mui/material';

import {EsTextInput} from '../../../shared/components/esTextInput';
import {WizardContext} from '../../../shared/context/wizard/WizardState';
import {StepProps, ValidationHandleWithSave} from '../Interfaces';

export const UserDetails = forwardRef<ValidationHandleWithSave, StepProps>(function UserDetails(props, ref) {
  const {t} = useTranslation();
  const {wizard, setUserDetails} = useContext(WizardContext);

  const {register, trigger, getValues, formState: {errors, isValid}} = useForm({
    mode: 'onBlur',
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
        const {username, password} = getValues();
        setUserDetails(username, password);
      }
    },
  }));

  const usernameRegister = register('username', {required: true});
  const passwordRegister = register('password', {required: true, minLength: 6});

  return (
    <Stack spacing={4}>
      <Typography variant="h4" color="primary">
        {t('WIZARD.ACCOUNT')}
      </Typography>
      <Typography variant="body1">
        {t('WIZARD.USERDETAILTEXT')}
      </Typography>
      <Box>
        <Box sx={{maxWidth: 250}}>
          <EsTextInput
            inputRef={usernameRegister}
            label={t('SETTINGS.ACCOUNT.USERNAME')}
            error={errors.username}
            defaultValue={getValues('username')}
            errorText={{
              required: t('SETTINGS.ACCOUNT.NOUSERNAME'),
            }}
          />
        </Box>
        <Box sx={{maxWidth: 250}}>
          <EsTextInput
            inputRef={passwordRegister}
            label={t('SETTINGS.PASSWORD.PASSWORD')}
            error={errors.password}
            type="password"
            defaultValue={getValues('password')}
            errorText={{
              required: t('SETTINGS.PASSWORD.NOPASSWORD'),
              minLength: t('SETTINGS.PASSWORD.PASSWORDMINLENGTH'),
            }}
          />
        </Box>
      </Box>
    </Stack>
  );
});
