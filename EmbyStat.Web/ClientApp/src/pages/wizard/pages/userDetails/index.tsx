import React, { ReactElement, useEffect, useState } from 'react';
import { Trans, useTranslation } from 'react-i18next';

import { Grid, Typography, TextField } from '@material-ui/core';
import { useSelector } from 'react-redux';
import { RootState } from '../../../../store/RootReducer';

interface Props {
  errors: any
  register: Function,
  disableBack: Function,
  disableNext: Function,
}

const UserDetails = (props: Props): ReactElement => {
  const { errors, register, disableBack, disableNext } = props;
  const [username, setUsername] = useState('');
  const { t } = useTranslation();

  useEffect(() => {
    disableBack(false);
  }, [disableBack]);

  useEffect(() => {
    disableNext(username.length === 0);
  }, [disableNext, username]);

  const handleChange = (event) => {
    setUsername(event.target.value);
  };

  const wizard = useSelector((state: RootState) => state.wizard);
  useEffect(() => {
    setUsername(wizard.username);
  }, [wizard])

  return (
    <Grid container direction="column">
      <Typography variant="h4" color="secondary">
        <Trans i18nKey="WIZARD.USERDETAILS" />
      </Typography>
      <Typography variant="body1" className="m-t-32">
        <Trans i18nKey="WIZARD.PROVIDEUSERNAME" />
      </Typography>
      <Grid container item xs={12} className="m-t-32">
        <TextField
          inputRef={register({ required: t('SETTINGS.GENERAL.NOUSERNAME') })}
          label={t('SETTINGS.GENERAL.USERNAME')}
          size="small"
          name="username"
          error={errors.username ? true : false}
          helperText={errors.username ? errors.username.message : ''}
          value={username}
          onChange={handleChange} />
      </Grid>
    </Grid>
  );
};

export default UserDetails;
