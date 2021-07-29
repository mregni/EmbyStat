
import React, { useState } from 'react'
import Grid from '@material-ui/core/Grid';

import { ActiveFilter, FilterDefinition } from '../../models/filter';
import { FilterItem } from '.';

interface Props {
  filters: FilterDefinition[];
  addFilter: (filter: ActiveFilter) => void;
  activeFilters: ActiveFilter[];
  clearFilter: (id: string) => void;
}

export const FilterContainer = (props: Props) => {
  const { filters, addFilter, activeFilters, clearFilter } = props;
  const [openedFilter, setOpenedFilter] = useState('');

  const openFilter = (id: string) => {
    setOpenedFilter(id);
  }

  return (
    <Grid container direction="row">
      {
        filters.map(filter => (
          <FilterItem
            opened={openedFilter}
            open={openFilter}
            filter={filter}
            save={addFilter}
            key={filter.id}
            activeFilter={activeFilters.filter(x => x.id === filter.id)[0]}
            clearFilter={clearFilter}
          />
        ))
      }
    </Grid>
  )
}
