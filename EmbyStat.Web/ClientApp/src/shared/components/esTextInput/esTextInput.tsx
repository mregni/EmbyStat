import React, { ReactElement } from 'react';
import TextField from '@material-ui/core/TextField';
import { makeStyles } from "@material-ui/core/styles";
import { FieldError } from 'react-hook-form';
import classNames from 'classnames';

const useStyles = makeStyles(() => ({
  error__placeholder: {
    marginBottom: 23,
  }
}));

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
  inputProps?: any
}

export const EsTextInput = (props: Props) => {
  const {
    error,
    inputRef,
    defaultValue,
    errorText,
    label = "",
    type = 'text',
    onChange,
    readonly = false,
    className = "",
    helperText = "",
    color = 'primary',
    inputProps = null
  } = props;
  const classes = useStyles();

  const { ref, ...rest } = inputRef;

  return (
    <TextField
      label={label}
      className={classNames(className, { [classes.error__placeholder]: !error && !helperText && errorText })}
      defaultValue={defaultValue}
      inputRef={ref}
      {...rest}
      variant="standard"
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
          onChange(event.target.value as string)
        }
      }}
      InputProps={inputProps}
    />
  )
}