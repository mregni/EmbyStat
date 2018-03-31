import { createSelector } from '@ngrx/store';
import { ApplicationState } from "../../states/app.state";

import { ServerInfoStore } from '../models/serverInfoStore';
import { ServerInfo } from '../models/serverInfo';
import { ServerActionTypes, ServerActions } from './actions.server';


const INITIAL_STATE: ServerInfoStore = {
  serverInfo: new ServerInfo(),
  isLoaded: false
};

export function serverInfoReducer(state: ServerInfoStore = INITIAL_STATE, action: ServerActions) {
  switch (action.type) {
    case ServerActionTypes.LOAD_SERVERINFO_SUCCESS:
    return {
      ...state,
      serverInfo: action.payload,
      isLoaded: true
    };
  default:
    return state;
  }
}

export namespace ServerQuery {
  export const getServerInfo = (state: ApplicationState) => state.serverInfo.serverInfo;
  export const getLoaded = (state: ApplicationState) => state.serverInfo.isLoaded;
}
