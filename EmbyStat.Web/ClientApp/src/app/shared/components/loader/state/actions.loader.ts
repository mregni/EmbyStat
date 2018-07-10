import { Action } from '@ngrx/store';

export enum LoaderActiontypes {
  SHOW = '[Loader] Show',
  HIDE = '[Loader] Hide'
}

export class HideLoader implements Action {
  readonly type = LoaderActiontypes.HIDE;
  constructor(public payload = null) { }
}

export class ShowLoader implements Action {
  readonly type = LoaderActiontypes.SHOW;
  constructor(public payload = null) { }
}

export type LoaderActions = ShowLoader | HideLoader;
