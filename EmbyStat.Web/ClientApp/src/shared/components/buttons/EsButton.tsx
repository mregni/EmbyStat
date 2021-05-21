import React from 'react';
import Button from '@material-ui/core/Button';
import CircularProgress from '@material-ui/core/CircularProgress';
import { useTranslation } from 'react-i18next';

export interface Props {
  disable: boolean;
  isSaving?: boolean;

}

const EsButton = (props: Props) => {
  const { disable, isSaving = false } = props;
  const { t } = useTranslation();

  return (
    <Button
      type="submit"
      color="primary"
      variant="contained"
      disabled={disable}
    >
      {isSaving ? (
        <CircularProgress color="inherit" size={22} />
      ) : (
          t('COMMON.SAVE')
        )}
    </Button>
  );
};

export default EsButton;
