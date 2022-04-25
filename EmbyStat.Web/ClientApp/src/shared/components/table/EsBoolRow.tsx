import React from 'react';
import {useTranslation} from 'react-i18next';

import CheckCircleOutlineIcon from '@mui/icons-material/CheckCircleOutline';
import HighlightOffIcon from '@mui/icons-material/HighlightOff';
import {TableCell, TableRow, Typography} from '@mui/material';

type EsBoolListItemProps = {
  label: string;
  value: boolean;
}

export function EsBoolRow(props: EsBoolListItemProps) {
  const {label, value} = props;
  const {t} = useTranslation();

  return (
    <TableRow>
      <TableCell sx={{width: 25}}>
        {value && (<CheckCircleOutlineIcon color="success" />)}
        {!value && (<HighlightOffIcon color="error" />)}
      </TableCell>
      <TableCell>
        <Typography>
          {t(label)}
        </Typography>
      </TableCell>
    </TableRow>
  );
}
