import React from 'react';

import {Chip} from '@mui/material';

import {useFilterHelpers} from '../../hooks';
import {ActiveFilter} from '../../models/filter';

type FilterChipProps = {
  filter: ActiveFilter;
  removeFilter: (id: string) => void;
}

export const EsFilterChip = (props: FilterChipProps) => {
  const {filter, removeFilter} = props;
  const {generateLabel} = useFilterHelpers();

  return (<Chip
    onDelete={() => removeFilter(filter.id)}
    label={generateLabel(filter)}
    variant="outlined"
  />);
};

