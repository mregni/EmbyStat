import React, { useEffect, useState } from 'react'
import { TextField } from '@material-ui/core';
import { useTranslation } from 'react-i18next';

import { FilterType } from '../../../models/filter';

interface Props {
  onValueChanged: (val0: string) => void;
  type: FilterType;
  errors: any
  register: Function,
  disableAdd: (disable: boolean) => void,
}

const FilterTextField = (props: Props) => {
  const { onValueChanged, type, errors, register, disableAdd } = props;
  const { t } = useTranslation();
  const [value, setValue] = useState('');

  useEffect(() => {
    disableAdd(value === '');
  }, [disableAdd, value])

  const handleChange = (event) => {
    setValue(event.target.value);
    disableAdd(errors.txt);
    if (!errors.txt) {
      onValueChanged(event.target.value);
    }
  };

  return (
    <form autoComplete="off">
      <TextField
        inputRef={register({ required: t('FORMERRORS.EMPTY') })}
        color="secondary"
        name={type.type}
        value={value}
        placeholder={t(type.placeholder ?? '')}
        error={errors.txt ? true : false}
        helperText={errors.txt ? errors.txt.message : ''}
        onChange={handleChange}
      />
    </form>
  )
}

export default FilterTextField
