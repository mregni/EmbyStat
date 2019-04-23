import { Injectable } from '@angular/core';

export interface MainMenuItems {
  shortLabel?: string;
  name: string;
  icon: string;
  route: string;
}

export interface Menu {
  label: string;
  items: MainMenuItems[];
}

const MENUITEMS = [
  {
    label: 'Statistics',
    items: [
      {
        shortLabel: 'S',
        name: 'Dashboard',
        icon: 'feather icon-home',
        route: ''
      },
      {
        shortLabel: 'M',
        name: 'Movies',
        icon: 'feather icon-film',
        route: '/movie/overview'
      },
      {
        shortLabel: 'M',
        name: 'Movies conainer',
        icon: 'feather icon-film',
        route: '/movie'
      }
    ]
  },
  {
    label: 'System',
    items: [
      {
        shortLabel: 'SE',
        name: 'Settings',
        icon: 'feather icon-settings',
        route: '/settings/general'
      },
      {
        shortLabel: 'A',
        name: 'About',
        icon: 'feather icon-help-circle',
        route: '/about'
      }
    ]
  }
];


@Injectable()
export class MenuItems {
  getAll(): Menu[] {
    return MENUITEMS;
  }
}
