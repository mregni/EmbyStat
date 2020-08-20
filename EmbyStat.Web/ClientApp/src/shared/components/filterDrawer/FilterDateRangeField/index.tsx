import React, { useState, useEffect } from 'react';
import MomentUtils from '@date-io/moment';
import moment, { Moment } from 'moment';
import {
  MuiPickersUtilsProvider,
  KeyboardDatePicker,
} from '@material-ui/pickers';
import { useTranslation } from 'react-i18next';
import Grid from '@material-ui/core/Grid';
import { makeStyles } from '@material-ui/core/styles';

const useStyles = makeStyles((theme) => ({
  small__fields: {
    width: 150,
    marginTop: 0,
  },
  'sub-text__padding': {
    marginTop: 5,
    fontStyle: 'italic',
  },
}));

interface Props {
  onValueChanged: (arg0: string) => void;
  errors: any;
  register: Function;
  disableAdd: (disable: boolean) => void;
}

interface BetweenValue {
  left: Moment | null;
  right: Moment | null;
}

const FilterDateRangeField = (props: Props) => {
  const { onValueChanged, errors, register, disableAdd } = props;
  const classes = useStyles();
  const { t } = useTranslation();
  const [betweenValue, setBetweenValue] = useState<BetweenValue>({
    left: null,
    right: null,
  });

  useEffect(() => {
    disableAdd(
      (!betweenValue.left?.isValid() ?? true) ||
      (!betweenValue.right?.isValid() ?? true)
    );
    onValueChanged(
      `${betweenValue.left?.format()}|${betweenValue.right?.format()}`
    );
  }, [betweenValue, onValueChanged, disableAdd]);

  const leftChanged = (date: Moment | null) => {
    if (date !== null && !errors.dateLeft) {
      setBetweenValue((state) => ({ ...state, left: date }));
    }
  };

  const rightChanged = (date: Moment | null) => {
    if (date !== null && !errors.dateRight) {
      setBetweenValue((state) => ({ ...state, right: date }));
    }
  };

  return (
    <Grid container direction="row" spacing={1}>
      <Grid item>
        <MuiPickersUtilsProvider utils={MomentUtils} locale={moment.locale()}>
          <KeyboardDatePicker
            margin="normal"
            format={moment().local().localeData().longDateFormat('L')}
            value={betweenValue.left}
            inputVariant="standard"
            autoOk
            onChange={leftChanged}
            className={classes.small__fields}
            name="dateLeft"
            error={!!errors.dateLeft}
            helperText={errors.dateLeft ? errors.dateLeft.message : ''}
            inputRef={register({ required: t('FORMERRORS.EMPTY') })}
          />
        </MuiPickersUtilsProvider>
      </Grid>
      <Grid item>
        <div className={classes['sub-text__padding']}>{t('COMMON.AND')}</div>
      </Grid>
      <Grid item>
        <MuiPickersUtilsProvider utils={MomentUtils} locale={moment.locale()}>
          <KeyboardDatePicker
            margin="normal"
            format={moment().local().localeData().longDateFormat('L')}
            value={betweenValue.right}
            inputVariant="standard"
            autoOk
            onChange={rightChanged}
            className={classes.small__fields}
            name="dateRight"
            error={!!errors.dateRight}
            helperText={errors.dateRight ? errors.dateRight.message : ''}
            inputRef={register({ required: t('FORMERRORS.EMPTY') })}
          />
        </MuiPickersUtilsProvider>
      </Grid>
    </Grid>
  );
};

export default FilterDateRangeField;
