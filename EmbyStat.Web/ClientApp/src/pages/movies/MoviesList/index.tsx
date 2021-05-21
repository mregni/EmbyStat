import React, { useState } from 'react';
import Grid from '@material-ui/core/Grid';
import Typography from '@material-ui/core/Typography';
import Zoom from '@material-ui/core/Zoom';
import Chip from '@material-ui/core/Chip';
import { useTranslation } from 'react-i18next';
import { makeStyles } from '@material-ui/core/styles';
import AddRoundedIcon from '@material-ui/icons/AddRounded';

import FilterDrawer from '../../../shared/components/filterDrawer';
import movieFilters from '../../../shared/filters/movieFilters';
import { ActiveFilter } from '../../../shared/models/filter';
import MovieTable from './MovieTable';
import MovieFilterDialog from './MovieFilterDialog';
import { FilterContainer } from "../../../shared/components/filter";

const useStyles = makeStyles(() => ({
  title: {
    marginRight: 16,
  },
}));

const MovieList = () => {
  const classes = useStyles();
  const { t } = useTranslation();
  const [activeFilters, setActiveFilters] = useState<ActiveFilter[]>([]);
  const [filterDialogOpen, setFilterDialogOpen] = useState(false);

  const handleFilterHide = (id) => {
    const currentFilterIndex = activeFilters.findIndex((x) => x.id === id);
    if (currentFilterIndex !== -1) {
      const newFilters = [...activeFilters];
      newFilters[currentFilterIndex].visible = false;
      setActiveFilters(newFilters);
    }
  };

  const addFilter = (filter: ActiveFilter) => {
    setActiveFilters((state) => [...state, filter]);
  };

  const openFilterDialog = () => {
    setFilterDialogOpen(true);
  }

  const clearFilter = (id: string) => {
    setActiveFilters(prev => prev.filter((x) => x.id !== id));
  }

  return (
    <Grid container direction="column">
      <Grid item container direction="row" className="p-b-16">
        <Grid item>
          <Typography variant="h5" className={classes.title}>
            {t('COMMON.MOVIES')}
          </Typography>
        </Grid>
        <FilterContainer
          filters={movieFilters}
          addFilter={addFilter}
          activeFilters={activeFilters}
          clearFilter={clearFilter}
        />
      </Grid>
      <Grid item container direction="row" justify="center" alignItems="center" className="max-height max-width">
        <MovieTable filters={activeFilters} />
      </Grid>
      <MovieFilterDialog
        setOpen={setFilterDialogOpen}
        open={filterDialogOpen}
      />
    </Grid>
  );
};

export default MovieList;
