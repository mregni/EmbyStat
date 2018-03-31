import { Action } from '@ngrx/store';
import { ServerInfo } from '../models/serverInfo';

export enum ServerActionTypes {
  LOAD_SERVERINFO = '[Server] Load ServerInfo',
  LOAD_SERVERINFO_SUCCESS = '[Server] Load ServerInfo Success'
}

export class LoadServerInfoAction implements Action {
  readonly type = ServerActionTypes.LOAD_SERVERINFO;
  constructor(public payload = null) { }
}

export class LoadServerInfoSuccessAction implements Action {
  readonly type = ServerActionTypes.LOAD_SERVERINFO_SUCCESS;
  constructor(public payload: ServerInfo) { }
}

export type ServerActions = LoadServerInfoAction | LoadServerInfoSuccessAction;
