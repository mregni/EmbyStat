import { Action } from '@ngrx/store';

import { ServerInfo } from '../../../shared/models/emby/server-info';

export enum EmbyServerActionTypes {
    LOAD_EMBY_SERVER_INFO = '[EmbyServerInfo] Load Info',
    LOAD_EMBY_SERVER_INFO_SUCCESS = '[EmbyServerInfo] Load Info Success',
    NOT_NEEDED = '[EmbyServerInfo] Not Needed'
}

export class LoadEmbyServerInfo implements Action {
    readonly type = EmbyServerActionTypes.LOAD_EMBY_SERVER_INFO;
    constructor(public payload = null) { }
}

export class LoadEmbyServerInfoSuccess implements Action {
    readonly type = EmbyServerActionTypes.LOAD_EMBY_SERVER_INFO_SUCCESS;
    constructor(public payload: ServerInfo) { }
}

export class NoNeedEmbyServerInfo implements Action {
    readonly type = EmbyServerActionTypes.NOT_NEEDED;
}

export type EmbyServerInfoActions = LoadEmbyServerInfo | LoadEmbyServerInfoSuccess | NoNeedEmbyServerInfo;
