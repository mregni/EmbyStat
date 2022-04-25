import React, {useState} from 'react';
import {useTranslation} from 'react-i18next';

import {Box} from '@mui/material';

import {EsTextInput} from '../../esTextInput';

interface Props {
  onValueChanged: (val0: string) => void;
  errors: any;
  register: Function;
}

export function EsFilterTextField(props: Props) {
  const {onValueChanged, errors, register} = props;
  const {t} = useTranslation();
  const [value, setValue] = useState('');

  const handleChange = (value: string) => {
    setValue(value);
    if (!errors.txt) {
      onValueChanged(value);
    }
  };

  const registerInput = register('text', {required: true});

  return (
    <Box sx={{width: '150px', pt: '3px'}}>
      <EsTextInput
        inputRef={registerInput}
        defaultValue={value}
        color="secondary"
        variant="standard"
        error={errors.txt}
        errorText={{required: t('FORMERRORS.EMPTY')}}
        onChange={handleChange}
      />
    </Box>
  );
}
