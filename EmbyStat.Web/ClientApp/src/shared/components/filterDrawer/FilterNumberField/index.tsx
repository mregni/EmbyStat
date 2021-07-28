import React, { useState, useEffect } from 'react';
import InputAdornment from '@material-ui/core/InputAdornment';
import { useTranslation } from 'react-i18next';

import { FilterType } from '../../../models/filter';
import { EsTextInput } from '../../esTextInput';

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

  const handleChange = (value: string) => {
    setValue(value);
    disableAdd(errors.number);
    if (!errors.number) {
      onValueChanged(value);
    }
  };

  const registerInput = register(type.type, { required: true });

  return (
    <EsTextInput
      inputRef={registerInput}
      defaultValue={value}
      color="secondary"
      type="number"
      error={errors.number}
      onChange={handleChange}
      errorText={{ required: t('FORMERRORS.EMPTY') }}
      inputProps={{
        endAdornment: (
          <InputAdornment position="end">{t(type.unit ?? '')}</InputAdornment>
        ),
      }}
    />
  );
};

export default FilterNumberField;
