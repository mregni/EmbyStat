import React, {useEffect, useState} from 'react';
import {useTranslation} from 'react-i18next';

import Grid from '@material-ui/core/Grid';
import InputAdornment from '@material-ui/core/InputAdornment';
import {makeStyles} from '@material-ui/core/styles';

import {FilterType} from '../../../models/filter';
import {EsTextInput} from '../../esTextInput';

const useStyles = makeStyles(() => ({
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

  const leftChanged = (value: string) => {
    if (!errors.betweenLeft) {
      setBetweenValue((state) => ({ ...state, left: value }));
    }
  };

  const rightChanged = (event) => {
    event.persist();
    if (!errors.betweenRight) {
      setBetweenValue((state) => ({ ...state, right: event.target.value }));
    }
  };

  const registerInputFrom = register(type.type, { required: true });
  const registerInputUntil = register(type.type, { required: true });

  return (
    <Grid container direction="row" spacing={1}>
      <Grid item>
        <EsTextInput
          inputRef={registerInputFrom}
          color="secondary"
          defaultValue=""
          type="number"
          errorText={{ required: t('FORMERRORS.REQUIRED') }}
          error={errors.betweenLeft}
          className={classes.small__fields}
          onChange={leftChanged}
          inputProps={{
            endAdornment: (
              <InputAdornment position="end">
                {t(type.unit ?? '')}
              </InputAdornment>
            )
          }}
        />
      </Grid>
      <Grid item>
        <div className={classes['sub-text__padding']}>{t('COMMON.AND')}</div>
      </Grid>
      <Grid item>
        <EsTextInput
          inputRef={registerInputUntil}
          color="secondary"
          defaultValue=""
          type="number"
          errorText={{ required: t('FORMERRORS.REQUIRED') }}
          error={errors.betweenRight}
          className={classes.small__fields}
          onChange={rightChanged}
          inputProps={{
            endAdornment: (
              <InputAdornment position="end">
                {t(type.unit ?? '')}
              </InputAdornment>
            )
          }}
        />
      </Grid>
    </Grid>
  );
};

export default FilterBetweenField;
