import React, {ReactElement} from 'react';
import {FieldError} from 'react-hook-form';

import {TextField} from '@mui/material';

export interface Props {
  error?: FieldError | undefined;
  inputRef: any;
  defaultValue: string | number | undefined | null;
  errorText?: any;
  label?: string;
  type?: 'text' | 'number' | 'password';
  onChange?: (value: string) => void;
  readonly?: boolean;
  className?: string;
  helperText?: string | ReactElement;
  color?: 'primary' | 'secondary',
  inputProps?: any,
  variant?: 'standard' | 'outlined'
}

export const EsTextInput = (props: Props) => {
  const {
    error,
    inputRef,
    defaultValue,
    errorText,
    label = '',
    type = 'text',
    onChange,
    readonly = false,
    className = '',
    helperText = '',
    color = 'primary',
    inputProps = null,
    variant = 'outlined',
  } = props;

  const {ref, ...rest} = inputRef;

  return (
    <TextField
      label={label}
      sx={{marginBottom: !error && !helperText && errorText !== null ? '23px !important' : 0}}
      className={className}
      defaultValue={defaultValue}
      inputRef={ref}
      {...rest}
      variant={variant}
      error={!!error}
      size="small"
      color={color}
      fullWidth
      autoComplete={'' + Math.random()}
      type={type}
      disabled={readonly}
      helperText={error ? errorText[error.type] : helperText}
      onChange={(event) => {
        if (!!onChange) {
          onChange(event.target.value as string);
        }
      }}
      InputProps={inputProps}
    />
  );
};
