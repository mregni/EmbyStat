import React, {Context, useContext, useState} from 'react';
import {useTranslation} from 'react-i18next';

import AddIcon from '@mui/icons-material/Add';
import {Chip, Stack} from '@mui/material';

import {LibraryContextProps} from '../../context/ILibraryContext';
import {ActiveFilter, FilterDefinition} from '../../models/filter';
import {EsFilterChip} from './EsFilterChip';
import {EsNewFilterDialog} from './EsNewFilterDialog';

type FilterProps<T> = {
  filters: FilterDefinition[];
  context: Context<LibraryContextProps<T>>;
}

export function EsFilterContainer<T, >(props: FilterProps<T>) {
  const {filters, context} = props;
  const {t} = useTranslation();
  const {activeFilters, addFilter, removeFilter} = useContext(context);
  const [open, setOpen] = useState(false);

  const closeDialog = () => {
    setOpen(false);
  };

  return (
    <Stack direction="row" spacing={1}>
      {
        activeFilters.map((filter: ActiveFilter) => (
          <EsFilterChip key={filter.id} filter={filter} removeFilter={removeFilter} />
        ))
      }
      <Chip
        icon={<AddIcon/>}
        label={t('COMMON.FILTER')}
        color="primary"
        variant="outlined"
        onClick={() => setOpen(true)}
      />
      <EsNewFilterDialog open={open} handleClose={closeDialog} filters={filters} addFilter={addFilter} />
    </Stack>
  );
}
