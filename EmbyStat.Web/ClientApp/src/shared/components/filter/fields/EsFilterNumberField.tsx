import React, {useState} from 'react';
import {useTranslation} from 'react-i18next';

import {Box, InputAdornment, Typography} from '@mui/material';

import {EsTextInput} from '../../esTextInput';

interface Props {
  onValueChanged: (val0: string) => void;
  errors: any;
  register: Function;
  unit?: string;
  fieldName?: string;
}

export function EsFilterNumberField(props: Props) {
  const {onValueChanged, errors, register, unit = '', fieldName = 'text'} = props;
  const {t} = useTranslation();
  const [value, setValue] = useState('');

  const handleChange = (value: string) => {
    setValue(value);
    if (!errors[fieldName]) {
      onValueChanged(value);
    }
  };

  const registerInput = register(fieldName, {required: true});

  return (
    <Box sx={{width: '50px', pt: '3px'}}>
      <EsTextInput
        inputRef={registerInput}
        defaultValue={value}
        color="secondary"
        variant="standard"
        error={errors[fieldName]}
        type='number'
        onChange={handleChange}
        inputProps={{
          endAdornment: (
            <InputAdornment position="end">
              <Typography color={errors[fieldName] !== undefined ? 'error': 'default'}>
                {t(unit)}
              </Typography>
            </InputAdornment>
          ),
        }}
      />
    </Box>
  );
}
