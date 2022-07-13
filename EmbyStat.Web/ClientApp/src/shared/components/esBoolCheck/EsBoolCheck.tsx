import React from 'react';

import CheckCircleOutlineIcon from '@mui/icons-material/CheckCircleOutline';
import HighlightOffIcon from '@mui/icons-material/HighlightOff';

type Props = {
  value: boolean
}

export function EsBoolCheck(props: Props) {
  const {value} = props;

  return value ?
    <CheckCircleOutlineIcon color="success" /> :
    <HighlightOffIcon color="error" />;
}
