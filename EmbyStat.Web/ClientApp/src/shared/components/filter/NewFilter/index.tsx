import React, { useState } from 'react'
import { makeStyles, Grid, TextField, Button } from '@material-ui/core';
import { useTranslation } from 'react-i18next'
import uuid from 'react-uuid';

import EmbyStatSelect from '../../../components/inputs/select/EmbyStatSelect';
import { FilterType } from '../../../models/filter';
import filters from './filters';

const useStyles = makeStyles((theme) => ({
  grid__text: {
  },
  grid__filter: {
    marginLeft: 25,
  },
  button__discard: {
    borderColor: theme.palette.error.main,
    color: theme.palette.error.main,
  }
}));

interface Props {
  discard: Function,
  save: Function,
}

function compare(a, b) {
  if (a.label < b.label) {
    return -1;
  }
  if (a.label > b.label) {
    return 1;
  }
  return 0;
}

const NewFilter = (props: Props) => {
  const { discard, save } = props;
  const classes = useStyles();
  const [sortedFiltersTypes] = useState(filters.sort(compare));
  const [selectedFilterType, setSelectedFilter] = useState<FilterType>(sortedFiltersTypes[0]);
  const [filterType, setFilterType] = useState(selectedFilterType.id);
  const [action, setAction] = useState(selectedFilterType.actions[0].value);
  const [value, setValue] = useState(selectedFilterType.type === 0 ? selectedFilterType.values[0].value : '');
  const { t } = useTranslation();

  const onFitlerChange = (event) => {
    const newFilter = sortedFiltersTypes.filter(x => x.id === event.target.value)[0];
    setSelectedFilter(newFilter);
    setFilterType(newFilter.id);
    setAction(newFilter.actions[0].value);
    if (newFilter.type === 0) {
      setValue(newFilter.values[0].value);
      return;
    }

    setValue('');
  }

  const onActionChange = (event) => {
    setAction(event.target.value);
  }

  const onValueChange = (event) => {
    setValue(event.target.value);
  }

  const prepareFilter = () => {
    const field = selectedFilterType.field;
    const label = selectedFilterType.label;
    const display = selectedFilterType.type === 0 ? selectedFilterType.values.filter(x => x.value === value)[0].label : value;
    save({ field, label, action, value, id: uuid(), display, visible: true });
  }


  return (
    <Grid
      container
      item
      direction="column"
      spacing={1}
      className="m-t-16">
      <Grid item container direction="column">
        <Grid item>
          {t('FILTERS.WHEREMOVIES')}
        </Grid>

        <Grid item className={classes.grid__filter}>
          <EmbyStatSelect
            variant="standard"
            value={filterType}
            onChange={onFitlerChange}
            menuItems={sortedFiltersTypes.map(x => ({ id: x.id, value: x.id, label: x.label }))}
          />
        </Grid>
        <Grid item className={classes.grid__filter}>
          <EmbyStatSelect
            variant="standard"
            value={action}
            onChange={onActionChange}
            menuItems={selectedFilterType.actions.map(x => ({ id: x.value, value: x.value, label: x.label }))}
          />
        </Grid>
        <Grid item className={classes.grid__filter}>
          {
            selectedFilterType.type === 0 ?
              <EmbyStatSelect
                variant="standard"
                value={value}
                onChange={onValueChange}
                menuItems={selectedFilterType.values.map(x => ({ id: x.value, value: x.value, label: x.label }))}
              />
              : <TextField
                value={value}
                variant="standard"
                placeholder={selectedFilterType.label}
                onChange={onValueChange}
              />
          }
        </Grid>
        <Grid item container spacing={1} justify="flex-end" className="m-t-16">
          <Grid item>
            <Button
              variant="outlined"
              size="small"
              classes={{
                outlined: classes.button__discard
              }}
              onClick={() => discard()}>{t('COMMON.DISCARD')}</Button>
          </Grid>
          <Grid item>
            <Button
              size="small"
              variant="contained"
              color="secondary"
              onClick={prepareFilter}>{t('COMMON.SAVE')}</Button>
          </Grid>
        </Grid>
      </Grid>
    </Grid>
  )
}

export default NewFilter
