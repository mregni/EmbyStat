import React, { useState, useEffect } from 'react'
import { TextField, InputAdornment } from '@material-ui/core';
import { useTranslation } from 'react-i18next';

import { FilterType } from '../../../models/filter';

interface Props {
  onValueChanged: (value: string) => void;
  type: FilterType;
  errors: any;
  register: Function;
  disableAdd: (disable: boolean) => void;
}

const FilterNumberField = (props: Props) => {
  const { onValueChanged, type, errors, register, disableAdd } = props;
  const { t } = useTranslation();
  const [value, setValue] = useState('');

  useEffect(() => {
    disableAdd(value === '');
  }, [disableAdd, value]);

  const handleChange = (event) => {
    setValue(event.target.value);
    disableAdd(errors.number);
    if (!errors.number) {
      onValueChanged(event.target.value);
    }
  };

  return (
    <TextField
      inputRef={register({ required: t('FORMERRORS.EMPTY') })}
      color="secondary"
      type="number"
      name={type.type}
      value={value}
      error={errors.number ? true : false}
      helperText={errors.number ? errors.number.message : ''}
      fullWidth
      onChange={handleChange}
      InputProps={{
        endAdornment: <InputAdornment position="end">{t(type.unit ?? "")}</InputAdornment>,
      }}
    />
  )
}

export default FilterNumberField
