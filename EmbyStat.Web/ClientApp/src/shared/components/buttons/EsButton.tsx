import {Button} from '@mui/material';
import React, {ReactElement} from 'react';

type Props = {
  disabled?: boolean;
  onClick: ((event: React.MouseEvent<HTMLButtonElement, MouseEvent>) => void) | undefined;
  children: ReactElement | ReactElement[];
  fullWidth?: boolean;
  variant?: 'text'| 'contained';
  color?: 'primary' |'secondary';
}

export const EsButton = (props: Props) => {
  const {
    disabled = false,
    fullWidth = true,
    children,
    onClick,
    variant = 'contained',
    color = 'primary',
  } = props;

  return (
    <Button
      type="submit"
      color={color}
      variant={variant}
      disabled={disabled}
      onClick={onClick}
      fullWidth={fullWidth}
    >
      {children}
    </Button>
  );
};
