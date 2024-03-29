import React from 'react';

import {Stack} from '@mui/material';

import {EsTitle} from '../../shared/components/esTitle';
import {EsFilterContainer} from '../../shared/components/filter';
import {ShowsContext} from '../../shared/context/shows';
import {showFilters} from './ShowFilters';
import {ShowTable} from './Table';

export function List() {
  return (
    <Stack direction="column" spacing={2}>
      <EsTitle content="COMMON.FILTERS" isFirst />
      <EsFilterContainer filters={showFilters} context={ShowsContext} />
      <ShowTable />
    </Stack>
  );
}
