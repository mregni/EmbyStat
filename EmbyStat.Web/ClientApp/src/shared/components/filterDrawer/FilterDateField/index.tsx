import React, { useState, useEffect } from 'react'
import MomentUtils from '@date-io/moment';
import moment, { Moment } from 'moment';
import {
  MuiPickersUtilsProvider,
  KeyboardDatePicker,
} from '@material-ui/pickers';
import { useTranslation } from 'react-i18next';

interface Props {
  onValueChanged: (arg0: string) => void;
  errors: any;
  register: Function;
  disableAdd: (disable: boolean) => void;
}


const FilterDateField = (props: Props) => {
  const { onValueChanged, errors, register, disableAdd } = props;
  const [selectedDate, setSelectedDate] = useState<Moment | null>(null);
  const { t } = useTranslation();

  useEffect(() => {
    disableAdd(!selectedDate?.isValid() ?? true);
  }, [disableAdd, selectedDate]);

  const handleDateChange = (date: Moment | null) => {
    setSelectedDate(date);
    if (!errors.date) {
      onValueChanged(date?.format() ?? '');
    }
  };

  return (
    <MuiPickersUtilsProvider utils={MomentUtils} locale={moment.locale()}>
      <KeyboardDatePicker
        margin="normal"
        format={moment().local().localeData().longDateFormat("L")}
        value={selectedDate}
        inputVariant="standard"
        autoOk
        onChange={handleDateChange}
        name="date"
        error={errors.date ? true : false}
        helperText={errors.date ? errors.date.message : ''}
        inputRef={register({ required: t('FORMERRORS.EMPTY') })}
      />
    </MuiPickersUtilsProvider>
  )
}

export default FilterDateField
