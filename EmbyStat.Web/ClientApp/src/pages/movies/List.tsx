import React from 'react';

import {Stack} from '@mui/material';

import {EsTitle} from '../../shared/components/esTitle';
import {EsFilterContainer} from '../../shared/components/filter';
import {MoviesContext} from '../../shared/context/movies';
import {movieFilters} from './MovieFilters';
import {MovieTable} from './Table';

export function List() {
  return (
    <Stack direction="column" spacing={2}>
      <EsTitle content="COMMON.FILTERS" isFirst />
      <EsFilterContainer filters={movieFilters} context={MoviesContext} />
      <MovieTable />
    </Stack>
  );
}
