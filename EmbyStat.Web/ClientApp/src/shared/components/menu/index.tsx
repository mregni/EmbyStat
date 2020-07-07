import React from 'react'
import { useTranslation } from "react-i18next";
import Drawer from '@material-ui/core/Drawer';
import List from '@material-ui/core/List';
import { makeStyles } from '@material-ui/core/styles';
import HomeRoundedIcon from '@material-ui/icons/HomeRounded';
import LocalMoviesRoundedIcon from '@material-ui/icons/LocalMoviesRounded';
import AssignmentRoundedIcon from '@material-ui/icons/AssignmentRounded';
import uuid from 'react-uuid';
import classNames from 'classnames';
import useMediaQuery from '@material-ui/core/useMediaQuery';

import theme from "../../../styles/theme";
import MenuItem from './MenuItem';

const drawerWidth = 240;
const useStyles = makeStyles((theme) => ({
  drawer: {
    flexShrink: 0,
  },
  drawerPaper: {
    width: drawerWidth,
    paddingTop: 32,
    [theme.breakpoints.up('sm')]: {
      paddingTop: 64,
    },
  },
  drawerContainer: {
    overflow: 'auto',
  },
  drawerOpen: {
    width: drawerWidth,
    transition: theme.transitions.create('width', {
      easing: theme.transitions.easing.sharp,
      duration: theme.transitions.duration.enteringScreen,
    }),
  },
  drawerClose: {
    transition: theme.transitions.create(['width', 'border'], {
      easing: theme.transitions.easing.sharp,
      duration: theme.transitions.duration.leavingScreen,
    }),
    overflowX: 'hidden',
    border: 'none',
    width: 0,
  },
  menu__list: {
    paddingTop: 0,
  },
}));

interface Props {
  open: boolean,
  setOpen: Function,
}

const Menu = (props: Props) => {
  const { open, setOpen } = props;
  const classes = useStyles();
  const { t } = useTranslation();
  const small = useMediaQuery(theme.breakpoints.down('md'));

  const menuItems = [
    {
      icon: <HomeRoundedIcon />,
      title: t('MENU.DASHBOARD'),
      route: '/',
    },
    {
      icon: <LocalMoviesRoundedIcon />,
      title: t('MENU.MOVIES'),
      children: [
        {
          title: t('COMMON.GENERAL'),
          route: '/movies/general',
        },
        {
          title: t('COMMON.PEOPLE'),
          route: '/movies/people',
        },
        {
          title: t('COMMON.GRAPHS'),
          route: '/movies/graphs',
        },
        {
          title: t('COMMON.TABLE'),
          route: '/movies/list',
        },
      ],
    },
    {
      icon: <AssignmentRoundedIcon />,
      title: t('MENU.JOBS'),
      route: '/jobs',
    },
  ];

  return (
    <Drawer
      className={classNames(classes.drawer, { [classes.drawerOpen]: open, [classes.drawerClose]: !open })}
      variant={small ? "temporary" : "permanent"}
      open={open}
      onClose={() => setOpen(false)}
      classes={{
        paper: classNames(classes.drawerPaper, { [classes.drawerOpen]: open, [classes.drawerClose]: !open }),
      }}>
      <div className={classes.drawerContainer}>
        {small ? <div>EmbyStat</div> : null}
        <List classes={{ root: classes.menu__list }}>
          {menuItems.map(item =>
            <MenuItem
              route={item.route}
              icon={item.icon}
              title={item.title}
              key={uuid()}
              setDrawerOpen={setOpen}
              drawerOpen={open}
              children={item.children} />
          )}
        </List>
      </div>
    </Drawer>
  )
}

export default Menu
