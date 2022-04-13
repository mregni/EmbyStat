import {t} from 'i18next';
import React from 'react';

import {TableCell, TableHead, TableRow} from '@mui/material';

import {EsSortLabel} from '../esSortLabel';
import {Header} from './';

type HeaderProps = {
  orderedBy?: string;
  order?: 'asc' | 'desc';
  sortHandler?: (property: string) => void;
  headers: Header[];
}


export const EsTableHeader = (props: HeaderProps) => {
  const {orderedBy = '', order = 'asc', sortHandler, headers} = props;
  return (
    <TableHead>
      <TableRow>
        {
          headers.map((column) =>
            <TableCell
              sx={{
                pt: 1,
                pb: 1,
              }}
              width={column.width}
              key={column.label}
              align={column.align ?? 'left'}
            >
              {
                column.sortable ?
                  (
                    <EsSortLabel
                      field={column.field ?? ''}
                      orderedBy={orderedBy}
                      order={order}
                      sortHandler={sortHandler}
                      label={column.label}
                    />
                  ) : (t(column.label))
              }
            </TableCell>,
          )
        }
      </TableRow>
    </TableHead>
  );
};
