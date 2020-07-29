import React from 'react';
import { useTranslation } from 'react-i18next';
import Drawer from '@material-ui/core/Drawer';
import List from '@material-ui/core/List';
import Grid from '@material-ui/core/Grid';
import { makeStyles } from '@material-ui/core/styles';
import HomeRoundedIcon from '@material-ui/icons/HomeRounded';
import LocalMoviesRoundedIcon from '@material-ui/icons/LocalMoviesRounded';
import AssignmentRoundedIcon from '@material-ui/icons/AssignmentRounded';
import SettingsIcon from '@material-ui/icons/Settings';
import uuid from 'react-uuid';
import classNames from 'classnames';
import useMediaQuery from '@material-ui/core/useMediaQuery';

import theme from '../../../styles/theme';
import MenuItem from './MenuItem';
import ServerStatus from './ServerStatus';
import { useSelector } from 'react-redux';
import { RootState } from '../../../store/RootReducer';
import { UserRoles } from '../../../shared/models/login';

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
  version: {
    color: theme.palette.grey[400],
    fontSize: "0.8rem",
    paddingBottom: 16
  }
}));

interface Props {
  open: boolean;
  setOpen: Function;
}

const Menu = (props: Props) => {
  const { open, setOpen } = props;
  const classes = useStyles();
  const { t } = useTranslation();
  const small = useMediaQuery(theme.breakpoints.down('md'));
  const settings = useSelector((state: RootState) => state.settings);


  const menuItems = [
    {
      icon: <HomeRoundedIcon />,
      title: t('MENU.DASHBOARD'),
      route: '/',
      roles: [UserRoles.Admin, UserRoles.User],
    },
    {
      icon: <LocalMoviesRoundedIcon />,
      title: t('MENU.MOVIES'),
      children: [
        {
          title: t('COMMON.GENERAL'),
          route: '/movies/general',
          roles: [UserRoles.Admin, UserRoles.User],
        },
        {
          title: t('COMMON.PEOPLE'),
          route: '/movies/people',
          roles: [UserRoles.Admin, UserRoles.User],
        },
        {
          title: t('COMMON.GRAPHS'),
          route: '/movies/graphs',
          roles: [UserRoles.Admin, UserRoles.User],
        },
        {
          title: t('COMMON.TABLE'),
          route: '/movies/list',
          roles: [UserRoles.Admin, UserRoles.User],
        },
      ],
    },
    {
      icon: <AssignmentRoundedIcon />,
      title: t('MENU.JOBS'),
      route: '/jobs',
      roles: [UserRoles.Admin, UserRoles.User],
    },
    {
      icon: <SettingsIcon />,
      title: t('MENU.SETTINGS'),
      children: [
        {
          title: t('COMMON.GENERAL'),
          route: '/settings/general',
          roles: [UserRoles.Admin, UserRoles.User],
        },
        {
          title: t('COMMON.MOVIES'),
          route: '/settings/movie',
          roles: [UserRoles.Admin, UserRoles.User],
        },
      ],
    },
  ];

  return (
    <Drawer
      className={classNames(classes.drawer, {
        [classes.drawerOpen]: open,
        [classes.drawerClose]: !open,
      })}
      variant={small ? 'temporary' : 'permanent'}
      open={open}
      onClose={() => setOpen(false)}
      classes={{
        paper: classNames(classes.drawerPaper, {
          [classes.drawerOpen]: open,
          [classes.drawerClose]: !open,
        }),
      }}
    >
      <Grid container direction="column" justify="space-between" className="max-height">
        <Grid item>
          <div className={classes.drawerContainer}>
            {small ? <div>EmbyStat</div> : null}
            <List classes={{ root: classes.menu__list }}>
              {menuItems.map((item) => (
                <MenuItem
                  roles={item.roles}
                  route={item.route}
                  icon={item.icon}
                  title={item.title}
                  key={uuid()}
                  setDrawerOpen={setOpen}
                  drawerOpen={open}
                  children={item.children}
                />
              ))}
            </List>
            <ServerStatus />
          </div>
        </Grid>
        <Grid item container justify="center" direction="row" className={classes.version}>
          {settings.version}
        </Grid>
      </Grid>
    </Drawer>
  );
};

export default Menu;
