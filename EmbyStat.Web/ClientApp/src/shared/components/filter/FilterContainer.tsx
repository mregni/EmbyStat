
import React, { useState } from 'react'
import Grid from '@material-ui/core/Grid';

import { FilterDefinition } from '../../models/filter';
import { FilterItem } from '.';

interface Props {
  filters: FilterDefinition[];
}

export const FilterContainer = (props: Props) => {
  const { filters } = props;
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
            key={filter.id}
          />
        ))
      }
    </Grid>
  )
}
