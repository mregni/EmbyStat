import React, { ReactNode, useState, useEffect } from 'react';
import ListItemIcon from '@material-ui/core/ListItemIcon';
import ListItemText from '@material-ui/core/ListItemText';
import ListItem from '@material-ui/core/ListItem';
import Collapse from '@material-ui/core/Collapse';
import { NavLink } from 'react-router-dom';
import ExpandLessRounded from '@material-ui/icons/ExpandLessRounded';
import ExpandMoreRounded from '@material-ui/icons/ExpandMoreRounded';
import { makeStyles } from '@material-ui/core/styles';
import useMediaQuery from '@material-ui/core/useMediaQuery';
import classNames from 'classnames';
import uuid from 'react-uuid';
import List from '@material-ui/core/List';

import theme from '../../../../styles/theme';

const useStyles = makeStyles((theme) => ({
  menu__item: (props: any) => ({
    height: 46,
    cursor: 'pointer',
    paddingLeft: theme.spacing(2),
    [theme.breakpoints.up('sm')]: {
      paddingLeft: props.isChild ? 48 + theme.spacing(3) : theme.spacing(3),
    },
  }),
  menu__icon: {
    minWidth: 0,
    marginRight: theme.spacing(2),
  },
  'link--active': {
    backgroundColor: 'rgba(255, 255, 255, 0.08)',
  },
  hide: {
    display: 'none',
  },
}));

interface Props {
  route: string | undefined;
  icon?: ReactNode;
  title: string;
  setDrawerOpen: Function;
  children: any;
  drawerOpen: boolean;
  isChild?: boolean;
}

const MenuItem = (props: Props) => {
  const {
    route,
    icon,
    title,
    setDrawerOpen,
    children,
    drawerOpen,
    isChild,
  } = props;
  const classes = useStyles({ isChild });
  const small = useMediaQuery(theme.breakpoints.down('md'));
  const [open, setOpen] = useState(false);

  useEffect(() => {
    if (!drawerOpen) {
      setOpen(false);
    }
  }, [drawerOpen]);

  const handleNavigation = () => {
    if (small) {
      setDrawerOpen(false);
    }
  };

  const openMenu = () => {
    if (!drawerOpen) {
      setDrawerOpen(true);
    }
    setOpen(!open);
  };

  return (
    <>
      {children != null && children.length > 0 ? (
        <>
          <ListItem classes={{ root: classes.menu__item }} onClick={openMenu}>
            <ListItemIcon classes={{ root: classes.menu__icon }}>
              {icon}
            </ListItemIcon>
            <ListItemText
              className={classNames({ [classes.hide]: !drawerOpen })}
              primary={title}
            />
            {children !== undefined && children.length > 0 && drawerOpen ? (
              open ? (
                <ExpandLessRounded />
              ) : (
                <ExpandMoreRounded />
              )
            ) : null}
          </ListItem>
          <Collapse in={open && drawerOpen} timeout="auto" unmountOnExit>
            <List component="div" disablePadding>
              {children.map((item) => (
                <MenuItem
                  route={item.route}
                  key={uuid()}
                  title={item.title}
                  setDrawerOpen={setDrawerOpen}
                  drawerOpen={drawerOpen}
                  children={null}
                  isChild={true}
                />
              ))}
            </List>
          </Collapse>
        </>
      ) : (
        <ListItem
          button
          component={NavLink}
          to={route as string}
          classes={{ root: classes.menu__item }}
          exact
          activeClassName={classes['link--active']}
          onClick={handleNavigation}
        >
          {icon != null ? (
            <ListItemIcon classes={{ root: classes.menu__icon }}>
              {icon}
            </ListItemIcon>
          ) : null}
          <ListItemText
            className={classNames({ [classes.hide]: !drawerOpen })}
            primary={title}
          />
        </ListItem>
      )}
    </>
  );
};

MenuItem.defaultProps = {
  isChild: false,
  icon: null,
};

export default MenuItem;
