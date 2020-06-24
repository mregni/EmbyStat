import React from 'react'
import { Button, CircularProgress } from '@material-ui/core';
import { useTranslation } from 'react-i18next';

interface Props {
  hasError: boolean,
  isSaving: boolean,
}

const SaveButton = (props: Props) => {
  const { hasError, isSaving } = props;
  const { t } = useTranslation();

  return (
    <Button type="submit" color="primary" variant="contained" disabled={hasError}>
      {isSaving
        ? <CircularProgress color="inherit" size={22} />
        : t('COMMON.SAVE')
      }
    </Button>
  )
}

export default SaveButton
