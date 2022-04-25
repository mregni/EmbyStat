import React from 'react';
import {useTranslation} from 'react-i18next';

import {TableSortLabel} from '@mui/material';
import {Box} from '@mui/system';

type Props = {
  field: string,
  orderedBy: string,
  order: 'asc' | 'desc' | undefined,
  sortHandler?: (sortField: string) => void,
  label: string,
}

export function EsSortLabel(props: Props) {
  const {field, orderedBy, order, sortHandler, label} = props;
  const {t} = useTranslation();

  const handleClick = () => {
    if (sortHandler !== undefined) {
      sortHandler(field);
    }
  };

  return (
    <TableSortLabel
      active={orderedBy === field}
      direction={orderedBy === field ? order : 'asc'}
      onClick={() => handleClick()}
    >
      {t(label)}
      {orderedBy === field ? (
        <Box sx={{
          border: 0,
          clip: 'rect(0 0 0 0)',
          height: 1,
          margin: -1,
          overflow: 'hidden',
          padding: 0,
          position: 'absolute',
          top: 20,
          width: 1,
        }}>
          {order === 'desc' ? 'sorted descending' : 'sorted ascending'}
        </Box>
      ) : null}
    </TableSortLabel>
  );
}
