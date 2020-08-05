import React, { useState, useEffect } from 'react';
import ListItem from '@material-ui/core/ListItem';
import ListItemText from '@material-ui/core/ListItemText';
import Collapse from '@material-ui/core/Collapse';
import RadioGroup from '@material-ui/core/RadioGroup';
import Grid from '@material-ui/core/Grid';
import Button from '@material-ui/core/Button';
import ExpandLessRounded from '@material-ui/icons/ExpandLessRounded';
import ExpandMoreRounded from '@material-ui/icons/ExpandMoreRounded';
import { useTranslation } from 'react-i18next';
import AddRoundedIcon from '@material-ui/icons/AddRounded';
import uuid from 'react-uuid';
import moment from 'moment';
import { useForm } from 'react-hook-form';

import {
  FilterDefinition,
  ActiveFilter,
  FilterType,
} from '../../../models/filter';
import FilterTextField from '../FilterTextField';
import FilterNumberField from '../FilterNumberField';
import FilterBetweenField from '../FilterBetweenField';
import FilterDropdownField from '../FilterDropdownField';
import FilterRadioButton from '../FilterRadioButton';
import FilterDateField from '../FilterDateField';
import FilterDateRangeField from '../FilterDateRangeField';

interface Props {
  save: (filter: ActiveFilter) => void;
  open: (id: string, state: boolean) => void;
  filterDefinition: FilterDefinition;
  setClickedId: Function;
}

const FilterPicker = (props: Props) => {
  const { save, open, filterDefinition, setClickedId } = props;
  const { t } = useTranslation();
  const [openState, setOpenState] = useState(false);
  const [clickedTypeId, setclickedTypeId] = useState();
  const [value, setValue] = useState<string>('');
  const [types, setTypes] = useState(filterDefinition.types);
  const [intputInError, setIntputInError] = useState(false);
  useEffect(() => {
    setOpenState(filterDefinition.open);
  }, [filterDefinition.open]);

  const handleClick = () => {
    setOpenState(!openState);
    setClickedId(filterDefinition.id);
  };

  const handleOpen = () => {
    if (openState) {
      open(filterDefinition.id, true);
    }
  };

  const changeFilterType = (id: string, state: boolean) => {
    if (clickedTypeId === id) {
      const current = types.filter((x) => x.id === id)[0];
      setTypes(
        types.map((x) =>
          x.id !== id
            ? { ...x, open: false }
            : current.open !== state
              ? { ...x, open: state }
              : x
        )
      );
    }

    reset();
  };

  const handleOnExited = () => {
    open(filterDefinition.id, false);
    setTypes((state) => state.map((x) => ({ ...x, open: false })));
  };

  const calculateValue = (): string | number => {
    switch (filterDefinition.field) {
      case 'RunTimeTicks':
        if (value.includes('|')) {
          return `${parseInt(value.split('|')[0], 10) * 600000000}|${parseInt(value.split('|')[1], 10) * 600000000}`;
        }
        return parseInt(value, 10) * 600000000;
      case 'Images':
      case 'Genres':
      case 'Container':
      case 'Subtitles':
        return `${value.split('|')[0]}`;
      default:
        return encodeURIComponent(value);
    }
  };

  const calculateValueLabel = (type: FilterType): string => {
    switch (type.type) {
      case 'dropdown':
        return `${value.split('|')[1]}`;
      case 'date':
        return moment(value).format('L');
      case 'dateRange':
        return `${encodeURI(moment(value.split('|')[0]).format('L'))} ${t(
          'COMMON.AND'
        )} ${encodeURI(moment(value.split('|')[1]).format('L'))}`;
      default:
        break;
    }

    switch (type.operation) {
      case 'between':
        return `${value.split('|')[0]}${
          type.unit != null ? t(type.unit) : ''
          } ${t('COMMON.AND')} ${value.split('|')[1]}${
          type.unit != null ? t(type.unit) : ''
          }`;
      case 'null':
        return '';
      default:
        return `${value}${type.unit != null ? t(type.unit) : ''}`;
    }
  };

  const { register, triggerValidation, errors, reset } = useForm({
    mode: 'onBlur',
    defaultValues: {
      txt: '',
    },
  });

  const saveFilter = async () => {
    if (await triggerValidation()) {
      const type = types.filter((x) => x.id === clickedTypeId)[0];
      const filter: ActiveFilter = {
        field: filterDefinition.field,
        fieldLabel: filterDefinition.label,
        operation: type.operation,
        operationLabel: type.label,
        value: calculateValue().toString(),
        valueLabel: calculateValueLabel(type),
        id: uuid(),
        visible: true,
        unit: type.unit,
      };

      save(filter);
    } else {
      setIntputInError(true);
    }
  };

  return (
    <>
      <ListItem button onClick={handleClick}>
        <ListItemText
          primary={filterDefinition.label.replace(/ \{[0-9]\}/g, '')}
        />
        {openState ? <ExpandLessRounded /> : <ExpandMoreRounded />}
      </ListItem>
      <Collapse
        in={openState}
        unmountOnExit
        onEnter={handleOpen}
        onExited={handleOnExited}
        className="p-l-32"
      >
        <Grid container direction="column" spacing={1}>
          <Grid item>
            <RadioGroup name="type">
              {types.map((type) => (
                <FilterRadioButton
                  key={type.id}
                  type={type}
                  open={changeFilterType}
                  setClickedTypeId={setclickedTypeId}
                >
                  <>
                    {type.type === 'txt' ? (
                      <FilterTextField
                        type={type}
                        onValueChanged={setValue}
                        errors={errors}
                        register={register}
                        disableAdd={setIntputInError}
                      />
                    ) : null}
                    {type.type === 'number' ? (
                      <FilterNumberField
                        type={type}
                        onValueChanged={setValue}
                        errors={errors}
                        register={register}
                        disableAdd={setIntputInError}
                      />
                    ) : null}
                    {type.type === 'range' ? (
                      <FilterBetweenField
                        type={type}
                        onValueChanged={setValue}
                        errors={errors}
                        register={register}
                        disableAdd={setIntputInError}
                      />
                    ) : null}
                    {type.type === 'dropdown' ? (
                      <FilterDropdownField
                        type={type}
                        onValueChanged={setValue}
                        field={filterDefinition.field}
                        disableAdd={setIntputInError}
                      />
                    ) : null}
                    {type.type === 'date' ? (
                      <FilterDateField
                        onValueChanged={setValue}
                        errors={errors}
                        register={register}
                        disableAdd={setIntputInError}
                      />
                    ) : null}
                    {type.type === 'dateRange' ? (
                      <FilterDateRangeField
                        onValueChanged={setValue}
                        errors={errors}
                        register={register}
                        disableAdd={setIntputInError}
                      />
                    ) : null}
                  </>
                </FilterRadioButton>
              ))}
            </RadioGroup>
          </Grid>
          <Grid item container direction="row" justify="flex-end">
            <Button
              variant="contained"
              color="secondary"
              disabled={clickedTypeId == null || intputInError}
              startIcon={<AddRoundedIcon />}
              className="m-b-8"
              onClick={saveFilter}
              size="small"
            >
              {t('COMMON.ADD')}
            </Button>
          </Grid>
        </Grid>
      </Collapse>
    </>
  );
};

export default FilterPicker;
