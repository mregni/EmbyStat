import React, {ReactElement} from 'react';

import HomeIcon from '@mui/icons-material/Home';
import InsertDriveFileIcon from '@mui/icons-material/InsertDriveFile';
import MovieIcon from '@mui/icons-material/Movie';
import SettingsIcon from '@mui/icons-material/Settings';
import StorageIcon from '@mui/icons-material/Storage';
import TvIcon from '@mui/icons-material/Tv';
import WorkIcon from '@mui/icons-material/Work';

export interface SimpleMenuItem {
  title: string;
  route: string;
}

export interface MenuItem extends SimpleMenuItem {
  icon: ReactElement;
  children?: SimpleMenuItem[];
}

export const MediaMenuItems: MenuItem[] = [
  {
    icon: <HomeIcon />,
    title: 'MENU.DASHBOARD',
    route: '/home',
  },
  {
    icon: <MovieIcon />,
    title: 'MENU.MOVIES',
    route: '/movies',
    children: [
      {
        title: 'COMMON.LIST',
        route: '/movies/list',
      },
    ],
  },
  {
    icon: <TvIcon />,
    title: 'MENU.SHOWS',
    route: '/shows',
    children: [
      {
        title: 'COMMON.LIST',
        route: '/shows/list',
      },
    ],
  },
  {
    icon: <StorageIcon />,
    title: 'MENU.SERVER',
    route: '/server',
  },
];

export const ServerMenuItems: MenuItem[] = [
  {
    icon: <WorkIcon />,
    title: 'MENU.JOBS',
    route: '/jobs',
  },
  {
    icon: <SettingsIcon />,
    title: 'MENU.SETTINGS',
    route: '/settings',
    children: [
      {
        title: 'MENU.MOVIES',
        route: '/settings/movie',
      },
      {
        title: 'MENU.SHOWS',
        route: '/settings/show',
      },
    ],
  },
  {
    icon: <InsertDriveFileIcon />,
    title: 'MENU.LOGS',
    route: '/logs',
  },
];
