import React, {useContext, useEffect, useState} from 'react';
import {useTranslation} from 'react-i18next';
import {useNavigate} from 'react-router';

import {
  Box, Divider, Drawer, List, ListItemButton, ListItemIcon, ListItemText, Stack, Toolbar,
  Typography,
} from '@mui/material';

import {SettingsContext} from '../../context/settings';
import {useEsLocation} from '../../hooks';
import {MediaMenuItems, MenuItem, ServerMenuItems, SimpleMenuItem} from './menuItems';

const drawerWidth = 200;

type MenuItemProps = {
  item: MenuItem;
}

type ChildMenuProps = {
  item: SimpleMenuItem;
}

function EsChildMenuItem(props: ChildMenuProps) {
  const {item} = props;
  const {t} = useTranslation();
  const navigate = useNavigate();
  const [isSelected, setIsSelected] = useState(false);

  const navigateToPage = () => {
    navigate(item.route);
  };

  useEffect(() => {
    setIsSelected(location.pathname.startsWith(item.route) );
  }, [item.route, location.pathname]);

  return (
    <ListItemButton
      key={item.route}
      selected={isSelected}
      onClick={navigateToPage}
      sx={{
        'pl': 8,
        'borderLeftStyle': 'solid',
        'borderLeftWidth': '8px',
        'borderLeftColor': (theme) => theme.palette.primary.main,
        'color': (theme) => theme.palette.primary.main,
        '&& .Mui-selected': {
          pl: (theme) => `${theme.spacing(8)} !important`,
        },
      }}
    >
      <ListItemText primary={t(item.title)} />
    </ListItemButton>
  );
}

function EsMenuItem(props: MenuItemProps) {
  const {item} = props;
  const {t} = useTranslation();
  const location = useEsLocation();
  const navigate = useNavigate();
  const [isSelected, setIsSelected] = useState(false);
  const [isSubItemSelected, setIsSubItemSelected] = useState(false);

  useEffect(() => {
    setIsSelected(location.pathname === item.route);
    setIsSubItemSelected(location.pathname !== item.route && location.pathname.startsWith(item.route));
  }, [item.route, location.pathname]);

  const navigateToPage = () => {
    navigate(item.route);
  };

  return (
    <>
      <ListItemButton
        selected={isSelected}
        onClick={navigateToPage}
        sx={{
          pl: 1,
          ...(isSubItemSelected && {
            borderLeftStyle: 'solid',
            borderLeftWidth: '8px',
            borderLeftColor: (theme) => theme.palette.primary.main,
            color: (theme) => theme.palette.primary.main,
          }),
        }}
      >
        <ListItemIcon>
          {item.icon}
        </ListItemIcon>
        <ListItemText primary={t(item.title)} />
      </ListItemButton>
      {location.pathname.startsWith(item.route) && ((item.children?.length ?? 0) > 0) ?
        item.children?.map((subItem) => (<EsChildMenuItem item={subItem} key={subItem.route} />)) :
        null}
    </>
  );
}

export function EsMenuDrawer() {
  const {systemConfig} = useContext(SettingsContext);

  return (
    <Drawer
      variant="permanent"
      sx={{
        'width': drawerWidth,
        'flexShrink': 0,
        '& .MuiDrawer-paper': {width: drawerWidth, boxSizing: 'border-box'},
      }}
    >
      <Stack
        justifyContent="space-between"
        sx={{height: '100%'}}
      >
        <Box>
          <Toolbar />
          <Box sx={{overflow: 'auto'}}>
            <List>
              {MediaMenuItems.map((item: MenuItem) => (<EsMenuItem key={item.route} item={item} />))}
            </List>
            <Divider />
            <List>
              {ServerMenuItems.map((item: MenuItem) => (<EsMenuItem key={item.route} item={item} />))}
            </List>
          </Box>
        </Box>
        <Box>
          <Typography
            variant='subtitle2'
            align='center'
            sx={{mb: 1}}>
            {systemConfig.version}
          </Typography>
        </Box>
      </Stack>
    </Drawer>
  );
}
