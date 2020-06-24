import React, { useState, useEffect } from 'react'
import { useTranslation } from 'react-i18next'
import { makeStyles, Button, Drawer, Grid, Typography, Chip, Zoom } from '@material-ui/core';
import SvgIcon from '@material-ui/core/SvgIcon';
import AddRoundedIcon from '@material-ui/icons/AddRounded';

import NewFilter from './NewFilter';

const useStyles = makeStyles((theme) => ({
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

interface Props {
  updateFilters: Function;
  filters: any[];
}

const Filter = (props: Props) => {
  const { updateFilters, filters } = props;
  const { t } = useTranslation();
  const [right, setRight] = useState(0);
  const classes = useStyles({ right: right });
  const [openFilters, setOpenFilters] = useState(false);

  const toggleDrawer = (open: boolean) => (
    event: React.KeyboardEvent | React.MouseEvent,
  ) => {
    if (event.type === 'keydown') {
      return;
    }

    setRight(382);
    setOpenFilters(open);
  };

  const saveFilter = (newFilter) => {
    updateFilters([...filters, newFilter]);
  }

  const list = () => (
    <Grid
      container
      className={classes.container}
      role="presentation"
      onKeyDown={toggleDrawer(false)}
      direction="column"
      spacing={2}
      justify="flex-start"
    >
      <Typography variant='h4'>{t('COMMON.FILTERS')}</Typography>
      <NewFilter
        discard={() => setOpenFilters(false)}
        save={saveFilter} />
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
        <SvgIcon
          classes={{
            root: classes.icon__root
          }}>
          <path d="M4.25,5.61C6.27,8.2,10,13,10,13v6c0,0.55,0.45,1,1,1h2c0.55,0,1-0.45,1-1v-6c0,0,3.72-4.8,5.74-7.39 C20.25,4.95,19.78,4,18.95,4H5.04C4.21,4,3.74,4.95,4.25,5.61z" />
        </SvgIcon>
      </Button>
      <Drawer anchor="right" open={openFilters} onClose={toggleDrawer(false)}>
        {list()}
      </Drawer>
    </>
  )
}

export default Filter
