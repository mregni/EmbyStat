import React, { ReactElement } from 'react';
import Button from '@material-ui/core/Button';

export interface Props {
  disabled?: boolean;
  onClick: ((event: React.MouseEvent<HTMLButtonElement, MouseEvent>) => void) | undefined;
  children: ReactElement | ReactElement[];
  fullWidth?: boolean;
}

export const EsButton = (props: Props) => {
  const {
    disabled = false,
    fullWidth = true,
    children,
    onClick
  } = props;

  return (
    <Button
      type="submit"
      color="primary"
      variant="contained"
      disabled={disabled}
      onClick={onClick}
      fullWidth={fullWidth}
    >
      {children}
    </Button>
  );
};
