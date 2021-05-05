import React, { useEffect, useState } from 'react'
import Grid from '@material-ui/core/Grid';
import { useTranslation } from 'react-i18next';
import KeyboardArrowDownRoundedIcon from '@material-ui/icons/KeyboardArrowDownRounded';
import ExpandLessRoundedIcon from '@material-ui/icons/ExpandLessRounded';
import Paper from '@material-ui/core/Paper';
import Collapse from '@material-ui/core/Collapse';
import { createStyles, makeStyles, Theme } from '@material-ui/core/styles';
import Popover from '@material-ui/core/Popover';
import ClickAwayListener from '@material-ui/core/ClickAwayListener';

import { FilterDefinition } from '../../models/filter';
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
    }
  }),
);

interface Props {
  filter: FilterDefinition;
  opened: string,
  open: (id: string) => void;
}

export const FilterItem = (props: Props) => {
  const { filter, opened, open } = props;
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

  return (
    <Grid item>
      <FilterDropdownButton
        variant="outlined"
        color="primary"
        onClick={clicked}
        endIcon={isOpen ? <ExpandLessRoundedIcon /> : <KeyboardArrowDownRoundedIcon />}
      >{t(filter.title)}</FilterDropdownButton>
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
              <RadioGroup name="type">
                {filter.types.map((type) => (
                  <FilterRadioButton
                    key={type.id}
                    type={type}
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
            </Grid>
            <Grid
              item
              container
              direction="row"
              justify="space-between"
              className={classes.button__container}
            >
              <Button variant="text">Clear all</Button>
              <Button variant="contained" color="primary">Add</Button>
            </Grid>
          </Grid>
        </Collapse>
      </Popover>
    </Grid>
  )
}
