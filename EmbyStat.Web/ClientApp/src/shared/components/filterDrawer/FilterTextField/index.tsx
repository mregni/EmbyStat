import React, { useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';

import { FilterType } from '../../../models/filter';
import { EsTextInput } from '../../esTextInput';

interface Props {
  onValueChanged: (val0: string) => void;
  type: FilterType;
  errors: any;
  register: Function;
  disableAdd: (disable: boolean) => void;
}

export const FilterTextField = (props: Props) => {
  const { onValueChanged, type, errors, register, disableAdd } = props;
  const { t } = useTranslation();
  const [value, setValue] = useState('');

  useEffect(() => {
    disableAdd(value === '');
  }, [disableAdd, value]);

  const handleChange = (value: string) => {
    setValue(value);
    disableAdd(errors.txt);
    if (!errors.txt) {
      onValueChanged(value);
    }
  };

  const registerInput = register(type.type, { required: true });

  return (
    <form autoComplete="off">
      <EsTextInput
        inputRef={registerInput}
        defaultValue={value}
        color="secondary"
        error={errors.txt}
        errorText={{ required: t('FORMERRORS.EMPTY') }}
        label={t(type.placeholder ?? '')}
        onChange={handleChange}
      />
    </form>
  );
};
