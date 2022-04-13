import React from 'react';
import {useTranslation} from 'react-i18next';
import {Button, CircularProgress} from '@mui/material';

type Props = {
  disabled?: boolean;
  isSaving: boolean;
  onClick?: ((event: React.MouseEvent<HTMLButtonElement, MouseEvent>) => void) | undefined;
}

export const EsSaveButton = (props: Props) => {
  const {
    disabled = false,
    isSaving = false,
    onClick = () => {},
  } = props;
  const {t} = useTranslation();

  return (
    <Button
      type="submit"
      color="primary"
      variant="contained"
      disabled={disabled}
      onClick={onClick}
    >
      {isSaving ? (
        <CircularProgress color="inherit" size={22} />
      ) : (
        t('COMMON.SAVE')
      )}
    </Button>
  );
};
