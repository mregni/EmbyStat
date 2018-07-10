import { Action } from '@ngrx/store';

export enum LoaderActiontypes {
  SHOWSHOWGENERAL = '[Loader] Show Show General',
  HIDESHOWGENERAL = '[Loader] Hide Show General',
  SHOWSHOWCHARTS = '[Loader] Show Show Carts',
  HIDEHOWCHARTS = '[Loader] Hide Show Carts'
}

export class HideLoaderShowGeneral implements Action {
  readonly type = LoaderActiontypes.HIDESHOWGENERAL;
  constructor(public payload = null) { }
}

export class ShowLoaderShowGeneral implements Action {
  readonly type = LoaderActiontypes.SHOWSHOWGENERAL;
  constructor(public payload = null) { }
}

export class ShowLoaderShowCharts implements Action {
  readonly type = LoaderActiontypes.SHOWSHOWCHARTS;
  constructor(public payload = null) { }
}

export class HideLoaderShowCharts implements Action {
  readonly type = LoaderActiontypes.HIDEHOWCHARTS;
  constructor(public payload = null) { }
}

export type LoaderActions = ShowLoaderShowGeneral | HideLoaderShowGeneral
  | ShowLoaderShowCharts | HideLoaderShowCharts;
