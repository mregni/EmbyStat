import React, { useState, useEffect } from 'react';
import Grid from '@material-ui/core/Grid';
import TextField from '@material-ui/core/TextField';
import InputAdornment from '@material-ui/core/InputAdornment';
import { makeStyles } from '@material-ui/core/styles';
import { useTranslation } from 'react-i18next';

import { FilterType } from '../../../models/filter';

const useStyles = makeStyles((theme) => ({
  small__fields: {
    width: 60,
  },
  'sub-text__padding': {
    marginTop: 5,
    fontStyle: 'italic',
  },
}));

interface Props {
  onValueChanged: (value: string) => void;
  type: FilterType;
  errors: any;
  register: Function;
  disableAdd: (disable: boolean) => void;
}

const FilterBetweenField = (props: Props) => {
  const { type, onValueChanged, errors, register, disableAdd } = props;
  const classes = useStyles();
  const { t } = useTranslation();
  const [betweenValue, setBetweenValue] = useState({
    left: '',
    right: '',
  });

  useEffect(() => {
    onValueChanged(`${betweenValue.left}|${betweenValue.right}`);
  }, [betweenValue, onValueChanged]);

  useEffect(() => {
    disableAdd(betweenValue.left === '' || betweenValue.right === '');
  }, [disableAdd, betweenValue]);

  const leftChanged = (event) => {
    event.persist();
    if (!errors.betweenLeft) {
      setBetweenValue((state) => ({ ...state, left: event.target.value }));
    }
  };

  const rightChanged = (event) => {
    event.persist();
    if (!errors.betweenRight) {
      setBetweenValue((state) => ({ ...state, right: event.target.value }));
    }
  };

  return (
    <Grid container direction="row" spacing={1}>
      <Grid item>
        <TextField
          inputRef={register({ required: t('FORMERRORS.REQUIRED') })}
          color="secondary"
          type="number"
          name="betweenLeft"
          value={betweenValue.left}
          error={!!errors.betweenLeft}
          helperText={errors.betweenLeft ? errors.betweenLeft.message : ''}
          fullWidth
          className={classes.small__fields}
          onChange={leftChanged}
          InputProps={{
            endAdornment: (
              <InputAdornment position="end">
                {t(type.unit ?? '')}
              </InputAdornment>
            ),
          }}
        />
      </Grid>
      <Grid item>
        <div className={classes['sub-text__padding']}>{t('COMMON.AND')}</div>
      </Grid>
      <Grid item>
        <TextField
          inputRef={register({ required: t('FORMERRORS.REQUIRED') })}
          color="secondary"
          type="number"
          name="betweenRight"
          value={betweenValue.right}
          error={!!errors.betweenRight}
          helperText={errors.betweenRight ? errors.betweenRight.message : ''}
          fullWidth
          className={classes.small__fields}
          onChange={rightChanged}
          InputProps={{
            endAdornment: (
              <InputAdornment position="end">
                {t(type.unit ?? '')}
              </InputAdornment>
            ),
          }}
        />
      </Grid>
    </Grid>
  );
};

export default FilterBetweenField;
