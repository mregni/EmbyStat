import React, { ReactElement, useEffect, useState } from 'react';
import { Trans, useTranslation } from 'react-i18next';
import Typography from '@material-ui/core/Typography';
import TextField from '@material-ui/core/TextField';
import Grid from '@material-ui/core/Grid';
import { useSelector } from 'react-redux';

import { RootState } from '../../../../store/RootReducer';

interface Props {
  errors: any;
  register: Function;
  disableBack: Function;
  disableNext: Function;
}

const UserDetails = (props: Props): ReactElement => {
  const { errors, register, disableBack, disableNext } = props;
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const { t } = useTranslation();

  useEffect(() => {
    disableBack(false);
  }, [disableBack]);

  useEffect(() => {
    disableNext(username.length === 0 || password.length === 0);
  }, [disableNext, username, password]);

  const wizard = useSelector((state: RootState) => state.wizard);
  useEffect(() => {
    setUsername(wizard.username);
    setPassword(wizard.password);
  }, [wizard]);

  console.log(errors);

  const getPasswordErrorMessage = (): string => {
    switch (errors.password.type) {
      case 'required':
        return t('SETTINGS.GENERAL.NOPASSWORD');
      case 'minLength':
        return t('SETTINGS.GENERAL.PASSWORDMINLENGTH');
      default:
        return '';
    }
  };

  return (
    <Grid container direction="column">
      <Typography variant="h4" color="primary">
        <Trans i18nKey="WIZARD.USERDETAILS" />
      </Typography>
      <Typography variant="body1" className="m-t-32">
        <Trans i18nKey="WIZARD.USERDETAILTEXT" />
      </Typography>
      <Grid
        container
        direction="row"
        item
        xs={12}
        className="m-t-32"
        spacing={4}
      >
        <Grid item xs={12} md={6}>
          <TextField
            inputRef={register({ required: t('SETTINGS.GENERAL.NOUSERNAME') })}
            label={t('SETTINGS.GENERAL.USERNAME')}
            size="small"
            name="username"
            error={!!errors.username}
            helperText={errors.username ? errors.username.message : ''}
            color="primary"
            value={username}
            onChange={(event) => setUsername(event.target.value as string)}
          />
        </Grid>
        <Grid item xs={12} md={6}>
          <TextField
            inputRef={register({ required: true, minLength: 6 })}
            label={t('SETTINGS.GENERAL.PASSWORD')}
            size="small"
            type="password"
            color="primary"
            name="password"
            error={!!errors.password}
            helperText={errors.password ? getPasswordErrorMessage() : ''}
            value={password}
            onChange={(event) => setPassword(event.target.value as string)}
          />
        </Grid>
      </Grid>
    </Grid>
  );
};

export default UserDetails;
