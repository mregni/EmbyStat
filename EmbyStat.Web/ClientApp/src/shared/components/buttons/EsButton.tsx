import React, {ReactElement} from 'react';

import {Button} from '@mui/material';

type Props = {
  disabled?: boolean;
  onClick: ((event: React.MouseEvent<HTMLButtonElement, MouseEvent>) => void) | undefined;
  children: ReactElement | ReactElement[];
  fullWidth?: boolean;
  variant?: 'text'| 'contained';
  color?: 'primary' |'secondary';
  type?: 'submit' | 'button';
}

export function EsButton(props: Props) {
  const {
    disabled = false,
    fullWidth = true,
    children,
    onClick,
    variant = 'contained',
    color = 'primary',
    type = 'submit',
  } = props;

  return (
    <Button
      type={type}
      color={color}
      variant={variant}
      disabled={disabled}
      onClick={onClick}
      fullWidth={fullWidth}
    >
      {children}
    </Button>
  );
}
