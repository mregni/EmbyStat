import React, { useContext } from 'react';
import { useTranslation } from 'react-i18next';
import Drawer from '@material-ui/core/Drawer';
import List from '@material-ui/core/List';
import Grid from '@material-ui/core/Grid';
import { makeStyles } from '@material-ui/core/styles';
import uuid from 'react-uuid';
import classNames from 'classnames';
import useMediaQuery from '@material-ui/core/useMediaQuery';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import { faHome, faFilm, faTv, faServer, faBriefcase, faCogs, faFileAlt } from '@fortawesome/free-solid-svg-icons'

import theme from '../../../styles/theme';
import MenuItem from './MenuItem';
import { UserRoles } from '../../../shared/models/login';
import { Typography } from '@material-ui/core';
import { SettingsContext } from '../../context/settings';

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
  const { settings } = useContext(SettingsContext);

  const menuItems = [
    {
      icon: <FontAwesomeIcon icon={faHome} />,
      title: t('MENU.DASHBOARD'),
      route: '/',
      roles: [UserRoles.Admin, UserRoles.User],
    },
    {
      icon: <FontAwesomeIcon icon={faFilm} />,
      title: t('MENU.MOVIES'),
      children: [
        {
          title: t('COMMON.GENERAL'),
          route: '/movies/general',
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
      icon: <FontAwesomeIcon icon={faTv} />,
      title: t('MENU.SHOWS'),
      children: [
        {
          title: t('COMMON.GENERAL'),
          route: '/shows/general',
          roles: [UserRoles.Admin, UserRoles.User],
        },
        {
          title: t('COMMON.GRAPHS'),
          route: '/shows/graphs',
          roles: [UserRoles.Admin, UserRoles.User],
        },
        {
          title: t('COMMON.TABLE'),
          route: '/shows/list',
          roles: [UserRoles.Admin, UserRoles.User],
        },
      ],
    },
    {
      icon: <FontAwesomeIcon icon={faServer} />,
      title: t('MENU.SERVER'),
      route: '/mediaserver',
      roles: [UserRoles.Admin, UserRoles.User],
    },
    {
      icon: <FontAwesomeIcon icon={faBriefcase} />,
      title: t('MENU.JOBS'),
      route: '/jobs',
      roles: [UserRoles.Admin, UserRoles.User],
    },
    {
      icon: <FontAwesomeIcon icon={faCogs} />,
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
    {
      icon: <FontAwesomeIcon icon={faFileAlt} />,
      title: t('MENU.LOGS'),
      route: '/logs',
      roles: [UserRoles.Admin, UserRoles.User],
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
            {small ? (
              <Grid item container justify="center">
                <Typography variant="h6" color="primary">
                  EmbyStat
                </Typography>
              </Grid>
            ) : null}
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
            {/* <ServerStatus /> */}
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
