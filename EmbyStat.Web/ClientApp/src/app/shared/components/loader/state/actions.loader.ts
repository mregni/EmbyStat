import { Action } from '@ngrx/store';

export enum LoaderActiontypes {
  SHOWSHOWGENERAL = '[Loader] Show Show General',
  HIDESHOWGENERAL = '[Loader] Hide Show General',
  SHOWSHOWCHARTS = '[Loader] Show Show Carts',
  HIDEHOWCHARTS = '[Loader] Hide Show Carts',
  SHOWSHOWCOLLECTION = '[Loader] Show Show Collection',
  HIDESHOWCOLLECTION = '[Loader] Hide Show Collection',
  SHOWMOVIEGENERAL = '[Loader] Show Movie General',
  HIDEMOVIEGENERAL = '[Loader] Hide Movie General',
  SHOWMOVIEGRAPHS = '[Loader] Show Movie Graphs',
  HIDEMOVIEGRAPHS = '[Loader] Hide Movie Graphs',
  SHOWMOVIEPEOPLE = '[Loader] Show Movie People',
  HIDEMOVIEPEOPLE = '[Loader] Hide Movie People',
  SHOWMOVIESUSPICIOUS = '[Loader] Show Movie Suspicious',
  HIDEMOVIESUSPICIOUS = '[Loader] Hide Movie Suspicious'
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

export class ShowLoaderShowCollection implements Action {
  readonly type = LoaderActiontypes.SHOWSHOWCOLLECTION;
  constructor(public payload = null) { }
}

export class HideLoaderShowCollection implements Action {
  readonly type = LoaderActiontypes.HIDESHOWCOLLECTION;
  constructor(public payload = null) { }
}

export class ShowLoaderMovieGeneral implements Action {
  readonly type = LoaderActiontypes.SHOWMOVIEGENERAL;
  constructor(public payload = null) { }
}

export class HideLoaderMovieGeneral implements Action {
  readonly type = LoaderActiontypes.HIDEMOVIEGENERAL;
  constructor(public payload = null) { }
}

export class ShowLoaderMovieGraphs implements Action {
  readonly type = LoaderActiontypes.SHOWMOVIEGRAPHS;
  constructor(public payload = null) { }
}

export class HideLoaderMovieGraphs implements Action {
  readonly type = LoaderActiontypes.HIDEMOVIEGRAPHS;
  constructor(public payload = null) { }
}

export class ShowLoaderMoviePeople implements Action {
  readonly type = LoaderActiontypes.SHOWMOVIEPEOPLE;
  constructor(public payload = null) { }
}

export class HideLoaderMoviePeople implements Action {
  readonly type = LoaderActiontypes.HIDEMOVIEPEOPLE;
  constructor(public payload = null) { }
}

export class ShowLoaderMovieSuspicious implements Action {
  readonly type = LoaderActiontypes.SHOWMOVIESUSPICIOUS;
  constructor(public payload = null) { }
}

export class HideLoaderMovieSuspicious implements Action {
  readonly type = LoaderActiontypes.HIDEMOVIESUSPICIOUS;
  constructor(public payload = null) { }
}

export type LoaderActions = ShowLoaderShowGeneral | HideLoaderShowGeneral
  | ShowLoaderShowCharts | HideLoaderShowCharts
  | ShowLoaderShowCollection | HideLoaderShowCollection
  | ShowLoaderMovieGeneral | HideLoaderMovieGeneral
  | ShowLoaderMovieGraphs | HideLoaderMovieGraphs
  | ShowLoaderMoviePeople | HideLoaderMoviePeople
  | ShowLoaderMovieSuspicious | HideLoaderMovieSuspicious;
