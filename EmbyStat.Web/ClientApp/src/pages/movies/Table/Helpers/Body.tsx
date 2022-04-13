import React from 'react';

import {TableBody} from '@mui/material';

import {TablePage} from '../../../../shared/models/common';
import {MovieRow} from '../../../../shared/models/movie';
import {Row} from './';

type BodyProps = {
  page: TablePage<MovieRow>;
}

export const Body = (props: BodyProps) => {
  const {page} = props;

  return (
    <TableBody>
      {page.data.map((row) => (<Row row={row} key={row.id} />))}
    </TableBody>
  );
};
