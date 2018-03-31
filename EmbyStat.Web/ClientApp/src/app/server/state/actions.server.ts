import { Action } from '@ngrx/store';
import { ServerInfo } from '../models/serverInfo';

export enum ServerActionTypes {
  LOAD_SERVERINFO = '[Server] Load ServerInfo',
  LOAD_SERVERINFO_SUCCESS = '[Server] Load ServerInfo Success',
  NOT_NEEDED = '[Server] Not Needed'
}

export class LoadServerInfoAction implements Action {
  readonly type = ServerActionTypes.LOAD_SERVERINFO;
  constructor(public payload = null) { }
}

export class LoadServerInfoSuccessAction implements Action {
  readonly type = ServerActionTypes.LOAD_SERVERINFO_SUCCESS;
  constructor(public payload: ServerInfo) { }
}

export class NoNeedServerInfoAction implements Action {
  readonly type = ServerActionTypes.NOT_NEEDED;
}

export type ServerActions = LoadServerInfoAction | LoadServerInfoSuccessAction | NoNeedServerInfoAction;
