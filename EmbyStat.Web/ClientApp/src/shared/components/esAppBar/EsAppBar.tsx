import {AppBar, Toolbar, IconButton, Menu, MenuItem, Stack, Typography} from '@mui/material';
import React, {useContext, useEffect, useState} from 'react';
import {AccountCircle} from '@mui/icons-material';
import {useTranslation} from 'react-i18next';
import ListItemText from '@mui/material/ListItemText';
import ListItemIcon from '@mui/material/ListItemIcon';
import LogoutIcon from '@mui/icons-material/Logout';
import {useNavigate} from 'react-router';

import {UserContext} from '../../context/user';
import SmallLogo from '../../../shared/assets/images/logo-small.png';

export const EsAppBar = () => {
  const [anchorEl, setAnchorEl] = React.useState<null | HTMLElement>(null);
  const {t} = useTranslation();
  const userContext = useContext(UserContext);
  const navigate = useNavigate();
  const [isLoggedIn, setIsLoggedIn] = useState(false);

  useEffect(() => {
    (async () => {
      const result = await userContext.isUserLoggedIn();
      setIsLoggedIn(result);
    })();
  }, []);

  const handleMenu = (event: React.MouseEvent<HTMLElement>) => {
    setAnchorEl(event.currentTarget);
  };

  const handleClose = () => {
    setAnchorEl(null);
  };

  const handleLogout = async () => {
    setAnchorEl(null);
    await userContext.logout();
    navigate('/login');
  };

  return (
    <AppBar position="fixed" sx={{zIndex: (theme) => theme.zIndex.drawer + 1}}>
      <Toolbar>
        <div style={{flexGrow: 1, paddingTop: 10}}>
          <img src={SmallLogo} width={180} alt="EmbyStat logo"/>
        </div>
        {isLoggedIn && (
          <Stack direction="row" alignItems="center">
            <Typography sx={{pr: 1}}>{userContext.user?.username}</Typography>
            <IconButton
              size="small"
              onClick={handleMenu}
              color="inherit"
            >
              <AccountCircle />
            </IconButton>
            <Menu
              id="menu-appbar"
              anchorEl={anchorEl}
              anchorOrigin={{
                vertical: 'top',
                horizontal: 'right',
              }}
              keepMounted
              transformOrigin={{
                vertical: 'top',
                horizontal: 'right',
              }}
              open={Boolean(anchorEl)}
              onClose={handleClose}
            >
              <MenuItem onClick={handleLogout}>
                <ListItemIcon>
                  <LogoutIcon fontSize="small" />
                </ListItemIcon>
                <ListItemText>{t('LOGIN.LOGOUT')}</ListItemText>
              </MenuItem>
            </Menu>
          </Stack>
        )}
      </Toolbar>
    </AppBar>
  );
};
