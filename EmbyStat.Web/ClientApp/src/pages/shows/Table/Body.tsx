import React from 'react';

import {TableBody} from '@mui/material';

import {TablePage} from '../../../shared/models/common';
import {ShowRow} from '../../../shared/models/show';
import {Row} from '.';

type BodyProps = {
  page: TablePage<ShowRow>;
};

export function Body(props: BodyProps) {
  const {page} = props;

  return (
    <TableBody>
      {page.data.map((row) => (<Row row={row} key={row.id} />))}
    </TableBody>
  );
}
