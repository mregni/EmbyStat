import { Action } from '@ngrx/store';
import { About } from '../models/about';

export enum AboutActionTypes {
  LOAD_ABOUT = '[About] Load About',
  LOAD_ABOUT_SUCCESS = '[About] Load About Success',
  NOT_NEEDED = '[About] Not Needed'
}

export class LoadAboutAction implements Action {
  readonly type = AboutActionTypes.LOAD_ABOUT;
  constructor(public payload = null) { }
}

export class LoadAboutSuccessAction implements Action {
  readonly type = AboutActionTypes.LOAD_ABOUT_SUCCESS;
  constructor(public payload: About) { }
}

export class NoNeedAboutAction implements Action {
  readonly type = AboutActionTypes.NOT_NEEDED;
}

export type AboutActions = LoadAboutAction | LoadAboutSuccessAction | NoNeedAboutAction;
