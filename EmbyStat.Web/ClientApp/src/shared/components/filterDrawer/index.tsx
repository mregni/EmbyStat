import React, { useState } from 'react'
import { useTranslation } from 'react-i18next'
import { makeStyles, Button, Drawer, Grid, Typography, Badge, List, ListItem, ListItemText } from '@material-ui/core';
import { Theme, withStyles, createStyles } from '@material-ui/core/styles';
import SvgIcon from '@material-ui/core/SvgIcon';
import uuid from 'react-uuid';

import { FilterDefinition, ActiveFilter } from '../../models/filter';
import FilterPicker from './FilterPicker';

const useStyles = makeStyles((theme: Theme) => ({
  container: {
    width: 350,
    margin: 16,
    height: '100%',
    position: 'relative',
  },
  icon__root: {
    color: theme.palette.getContrastText(theme.palette.primary.main),
  },
  button__root: {
    width: 40,
    height: 50,
    borderTopRightRadius: 0,
    borderBottomRightRadius: 0,
    position: 'fixed',
    right: 0,
    top: 150,
    zIndex: 5000,
  },
}));

const StyledBadge = withStyles((theme: Theme) =>
  createStyles({
    badge: {
      right: -3,
      top: 18,
      border: `2px solid ${theme.palette.background.paper}`,
      padding: '0 4px',
    },
  }),
)(Badge);

interface Props {
  addFilter: (filter: ActiveFilter) => void;
  filterDefinitions: FilterDefinition[];
  filterCount: number;
  clearFilters: () => void;
}

const FilterDrawer = (props: Props) => {
  const { clearFilters, addFilter, filterDefinitions, filterCount } = props;
  const classes = useStyles();
  const { t } = useTranslation();
  const [openFiltersDrawer, setOpenFiltersDrawer] = useState(false);
  const [definitions, setDefinitions] = useState(filterDefinitions);
  const [lastClickedDefinition, setLastClickedDefinition] = useState();

  const toggleDrawer = (open: boolean) => (
    event: React.KeyboardEvent | React.MouseEvent,
  ) => {
    if (event.type === 'keydown') {
      return;
    }
    setOpenFiltersDrawer(open);
    definitions.forEach(x => x.open = false);
  };

  const openFilterDefinition = (id: string, state: boolean) => {
    if (lastClickedDefinition === id) {
      const current = definitions.filter(x => x.id === id)[0];
      setDefinitions(definitions.map((x) => (x.id !== id ? { ...x, open: false } : current.open !== state ? { ...x, open: state } : x)))
    }
  }

  const list = () => (
    <Grid
      container
      className={classes.container}
      role="presentation"
      onKeyDown={toggleDrawer(false)}
      direction="column"
      spacing={1}
      justify="flex-start"
    >
      <Grid item container justify={filterCount > 0 ? "space-between" : "flex-start"}>
        <Grid item>
          <Typography variant='h4'>
            {t('COMMON.FILTERS')}
          </Typography>
        </Grid>
        {filterCount > 0 ?
          <Grid item>
            <Button color="secondary" onClick={() => clearFilters()}>Clear All</Button>
          </Grid>
          : null}
      </Grid>
      <Grid item>
        <hr />
      </Grid>
      <Grid item container direction="column">
        <List>
          {definitions.map((filterDefinition: FilterDefinition) =>
            <FilterPicker
              filterDefinition={filterDefinition}
              open={openFilterDefinition}
              save={addFilter}
              key={filterDefinition.id}
              setClickedId={setLastClickedDefinition}
            />
          )}
        </List>
      </Grid>
    </Grid >
  );

  return (
    <>
      <Button
        onClick={toggleDrawer(true)}
        variant="contained"
        size="small"
        color="primary"
        classes={{
          root: classes.button__root
        }}>
        <StyledBadge badgeContent={filterCount} color="secondary">
          <SvgIcon
            classes={{
              root: classes.icon__root
            }}>
            <path d="M4.25,5.61C6.27,8.2,10,13,10,13v6c0,0.55,0.45,1,1,1h2c0.55,0,1-0.45,1-1v-6c0,0,3.72-4.8,5.74-7.39 C20.25,4.95,19.78,4,18.95,4H5.04C4.21,4,3.74,4.95,4.25,5.61z" />
          </SvgIcon>
        </StyledBadge>
      </Button>
      <Drawer anchor="right" open={openFiltersDrawer} onClose={toggleDrawer(false)}>
        {list()}
      </Drawer>
    </>
  )
}

export default FilterDrawer
