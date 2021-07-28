import React, { useEffect, useState, ReactNode } from 'react';
import { makeStyles } from '@material-ui/core/styles';
import Grid from '@material-ui/core/Grid';
import FormControlLabel from '@material-ui/core/FormControlLabel';
import Radio from '@material-ui/core/Radio';
import Collapse from '@material-ui/core/Collapse';

import { useTranslation } from 'react-i18next';
import { FilterType } from '../../../models/filter';

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
  isOpen: boolean;
  open: Function;
  setClickedTypeId: Function;
  children: ReactNode;
}

const FilterRadioButton = (props: Props) => {
  const { type, open, setClickedTypeId, children, isOpen } = props;
  const classes = useStyles();
  const { t } = useTranslation();
  const [openState, setOpenState] = useState(false);

  useEffect(() => {
    setOpenState(isOpen);
  }, [isOpen])

  useEffect(() => {
    setOpenState(type.open);
  }, [type.open]);

  const handleClick = () => {
    setOpenState(prev => !prev);
    setClickedTypeId(type.id);
  };

  const handleOpen = () => {
    if (openState) {
      open(type.id, true);
    }
  };

  return (
    <Grid container direction="column">
      <Grid item>
        <FormControlLabel
          key={type.id}
          value={type.id}
          control={<Radio color="primary" />}
          label={t(type.label)}
          onClick={handleClick}
        />
      </Grid>
      <Grid item>
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
  );
};

export default FilterRadioButton;
