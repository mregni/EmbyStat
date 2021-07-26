import React from 'react';
import Button from '@material-ui/core/Button';
import CircularProgress from '@material-ui/core/CircularProgress';
import { useTranslation } from 'react-i18next';

export interface Props {
  disabled?: boolean;
  isSaving: boolean;
  onClick: ((event: React.MouseEvent<HTMLButtonElement, MouseEvent>) => void) | undefined;
}

export const EsSaveButton = (props: Props) => {
  const {
    disabled = false,
    isSaving = false,
    onClick
  } = props;
  const { t } = useTranslation();

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
