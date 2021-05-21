import React, { useEffect, useState } from 'react'
import Grid from '@material-ui/core/Grid';
import { useTranslation } from 'react-i18next';
import KeyboardArrowDownRoundedIcon from '@material-ui/icons/KeyboardArrowDownRounded';
import ExpandLessRoundedIcon from '@material-ui/icons/ExpandLessRounded';
import Collapse from '@material-ui/core/Collapse';
import { createStyles, makeStyles, Theme } from '@material-ui/core/styles';
import Popover from '@material-ui/core/Popover';
import classNames from 'classnames';

import { ActiveFilter, FilterDefinition, FilterType } from '../../models/filter';
import { FilterDropdownButton } from './FilterDropdownButton';
import Button from '@material-ui/core/Button';
import RadioGroup from '@material-ui/core/RadioGroup';
import FilterRadioButton from '../filterDrawer/FilterRadioButton';
import FilterTextField from '../filterDrawer/FilterTextField';
import FilterNumberField from '../filterDrawer/FilterNumberField';
import FilterBetweenField from '../filterDrawer/FilterBetweenField';
import FilterDateField from '../filterDrawer/FilterDateField';
import FilterDropdownField from '../filterDrawer/FilterDropdownField';
import FilterDateRangeField from '../filterDrawer/FilterDateRangeField';
import { useForm } from 'react-hook-form';
import moment from 'moment';

const useStyles = makeStyles((theme: Theme) =>
  createStyles({
    popper__root: {
      marginTop: 4
    },
    popper__container: {
      margin: theme.spacing(1),
      width: 300,
      padding: 8
    },
    button__container: {
      borderTop: "1px solid #535353",
      paddingTop: 16
    },
    content__container: {
      padding: "0 0 16px 12px"
    },
    active__filter: {
      color: "#97e733",
      border: "1px solid #97e733"
    }
  }),
);

interface Props {
  filter: FilterDefinition;
  opened: string,
  open: (id: string) => void;
  save: (filter: ActiveFilter) => void;
  activeFilter: ActiveFilter | undefined;
  clearFilter: (id: string) => void;
}

export const FilterItem = (props: Props) => {
  const { filter, opened, open, save, activeFilter, clearFilter } = props;
  const classes = useStyles();
  const { t } = useTranslation();
  const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);
  const [isOpen, setIsOpen] = useState(false);

  useEffect(() => {
    setIsOpen(opened === filter.id);
    if (opened !== filter.id) {
      setAnchorEl(null);
    }
  }, [opened, filter]);

  const clicked = (event: React.MouseEvent<HTMLElement>) => {
    open(opened !== filter.id ? filter.id : '');
    setAnchorEl(event.currentTarget);
  }

  const handleClose = () => {
    open('');
  };

  const id = opened ? 'transitions-popper' : undefined;

  const { register, trigger, errors, reset } = useForm({
    mode: "onBlur",
    defaultValues: {
      txt: "",
    },
  });

  const [clickedTypeId, setclickedTypeId] = useState();
  const [types, setTypes] = useState(filter.types);
  const [value, setValue] = useState<string>("");
  const [intputInError, setIntputInError] = useState(false);
  const [selectedRadio, setSelectedRadio] = useState<string>('');

  const changeFilterType = (id: string, state: boolean) => {
    if (clickedTypeId === id) {
      const current = filter.types.filter((x) => x.id === id)[0];
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

  const calculateValue = (): string | number => {
    switch (filter.field) {
      case "RunTimeTicks":
        if (value.includes("|")) {
          return `${parseInt(value.split("|")[0], 10) * 600000000}|${parseInt(value.split("|")[1], 10) * 600000000
            }`;
        }
        return parseInt(value, 10) * 600000000;
      case "Images":
      case "Genres":
      case "Container":
      case "Subtitles":
      case "Codec":
      case "VideoRange":
        return `${value.split("|")[0]}`;
      default:
        return encodeURIComponent(value);
    }
  };

  const calculateValueLabel = (type: FilterType): string => {
    switch (type.type) {
      case "dropdown":
        return `${value.split("|")[1]}`;
      case "date":
        return moment(value).format("L");
      case "dateRange":
        return `${encodeURI(moment(value.split("|")[0]).format("L"))} ${t(
          "COMMON.AND"
        )} ${encodeURI(moment(value.split("|")[1]).format("L"))}`;
      default:
        break;
    }

    switch (type.operation) {
      case "between":
        return `${value.split("|")[0]}${type.unit != null ? t(type.unit) : ""
          } ${t("COMMON.AND")} ${value.split("|")[1]}${type.unit != null ? t(type.unit) : ""
          }`;
      case "null":
        return "";
      default:
        return `${value}${type.unit != null ? t(type.unit) : ""}`;
    }
  };

  const onClearFilter = () => {
    if (activeFilter) {
      clearFilter(activeFilter.id);
    } else {
      setSelectedRadio('');
      setTypes(prev => prev.map((x) => ({ ...x, open: false })));
      reset();
    }

    open('');
  }

  const addFilter = async () => {
    if (await trigger()) {
      const type = types.filter((x) => x.id === clickedTypeId)[0];
      const activeFilter: ActiveFilter = {
        field: filter.field,
        fieldLabel: filter.label,
        operation: type.operation,
        operationLabel: type.label,
        value: calculateValue().toString(),
        valueLabel: calculateValueLabel(type),
        id: filter.id,
        visible: true,
        unit: type.unit,
      };

      save(activeFilter);
    }
    trigger();
    onClearFilter();
  }

  const generateLabel = (filter: ActiveFilter): string => {
    return filter.fieldLabel
      .replace(/\{0\}/g, t(filter.operationLabel))
      .replace(/\{1\}/g, filter.valueLabel);
  };

  return (
    <Grid item>
      <FilterDropdownButton
        classes={{ root: classNames({ [classes.active__filter]: activeFilter !== undefined }) }}
        variant="outlined"
        color="primary"
        onClick={clicked}
        endIcon={isOpen ? <ExpandLessRoundedIcon /> : <KeyboardArrowDownRoundedIcon />}
      >
        {t(filter.title)}
      </FilterDropdownButton>
      <Popover
        id={id}
        open={isOpen}
        anchorEl={anchorEl}
        onClose={handleClose}
        classes={{
          root: classes.popper__root
        }}
        anchorOrigin={{
          vertical: 'bottom',
          horizontal: 'center',
        }}
        transformOrigin={{
          vertical: 'top',
          horizontal: 'center',
        }}
      >
        <Collapse in={isOpen} unmountOnExit>
          <Grid container direction="column" className={classes.popper__container} >
            <Grid container direction="column" spacing={1} className={classes.content__container}>
              {
                activeFilter ? (
                  <Grid item>
                    {generateLabel(activeFilter)}
                  </Grid>
                ) :
                  (
                    <RadioGroup
                      name={filter.title}
                      value={selectedRadio}
                      onChange={(event) => setSelectedRadio(event.target.value)}
                    >
                      {filter.types.map((type) => (
                        <FilterRadioButton
                          key={type.id}
                          type={type}
                          isOpen={type.id === selectedRadio}
                          open={changeFilterType}
                          setClickedTypeId={setclickedTypeId}
                        >
                          <>
                            {type.type === "txt" ? (
                              <FilterTextField
                                type={type}
                                onValueChanged={setValue}
                                errors={errors}
                                register={register}
                                disableAdd={setIntputInError}
                              />
                            ) : null}
                            {type.type === "number" ? (
                              <FilterNumberField
                                type={type}
                                onValueChanged={setValue}
                                errors={errors}
                                register={register}
                                disableAdd={setIntputInError}
                              />
                            ) : null}
                            {type.type === "range" ? (
                              <FilterBetweenField
                                type={type}
                                onValueChanged={setValue}
                                errors={errors}
                                register={register}
                                disableAdd={setIntputInError}
                              />
                            ) : null}
                            {type.type === "dropdown" ? (
                              <FilterDropdownField
                                type={type}
                                onValueChanged={setValue}
                                field={filter.field}
                                disableAdd={setIntputInError}
                              />
                            ) : null}
                            {type.type === "date" ? (
                              <FilterDateField
                                onValueChanged={setValue}
                                errors={errors}
                                register={register}
                                disableAdd={setIntputInError}
                              />
                            ) : null}
                            {type.type === "dateRange" ? (
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
                  )}
            </Grid>
            <Grid
              item
              container
              direction="row"
              justify="space-between"
              className={classes.button__container}
            >
              <Button variant="text" onClick={() => onClearFilter()}>Clear</Button>
              <Button
                variant="contained"
                color="primary"
                onClick={() => addFilter()}
                disabled={activeFilter !== undefined}
              >Add</Button>
            </Grid>
          </Grid>
        </Collapse>
      </Popover>
    </Grid >
  )
}
