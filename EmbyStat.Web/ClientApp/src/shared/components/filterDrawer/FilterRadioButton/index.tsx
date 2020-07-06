import React, { useEffect, useState, ReactNode } from 'react'
import { makeStyles, Grid, FormControlLabel, Radio, Collapse } from '@material-ui/core';
import { FilterType } from '../../../models/filter';
import { useTranslation } from 'react-i18next';

const useStyles = makeStyles((theme) => ({
  textfield__root: {
    marginBottom: 8,
    marginLeft: 30,
    marginTop: -7,
    marginRight: 22,
  },
}));

interface Props {
  type: FilterType;
  open: Function;
  setClickedTypeId: Function;
  children: ReactNode;
}

const FilterRadioButton = (props: Props) => {
  const { type, open, setClickedTypeId, children } = props;
  const classes = useStyles();
  const { t } = useTranslation();
  const [openState, setOpenState] = useState(false);


  useEffect(() => {
    setOpenState(type.open);
  }, [type.open]);

  const handleClick = () => {
    setOpenState(true);
    setClickedTypeId(type.id);
  }

  const handleOpen = () => {
    if (openState) {
      open(type.id, true)
    }
  }

  return (
    <Grid container direction="column">
      <Grid item>
        <FormControlLabel
          key={type.id}
          value={type.id}
          control={<Radio />}
          label={t(type.label)}
          onClick={handleClick}
        />
      </Grid>
      <Grid item >
        <Collapse
          in={openState}
          unmountOnExit
          onEnter={handleOpen}
          onExited={() => open(type.id, false)}
          className={classes.textfield__root}
        >
          {children}
        </Collapse>
      </Grid>
    </Grid>
  )
}

export default FilterRadioButton
