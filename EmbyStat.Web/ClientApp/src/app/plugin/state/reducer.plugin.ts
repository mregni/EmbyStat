import { createSelector } from '@ngrx/store';
import { ApplicationState } from "../../states/app.state";

import { EmbyPluginStore } from '../models/embyPluginStore';
import { PluginActionTypes, PluginActions } from './actions.plugin';


const INITIAL_STATE: EmbyPluginStore = {
  list: [],
  isLoaded: false
};

export function pluginReducer(state: EmbyPluginStore = INITIAL_STATE, action: PluginActions) {
  switch (action.type) {
  case PluginActionTypes.LOAD_PLUGINS_SUCCESS:
    return {
      ...state,
      list: action.payload,
      isLoaded: true
      };
    case PluginActionTypes.RESET_LOADED_STATE:
      return {
        ...state,
        isLoaded: false
      };
  default:
    return state;
  }
}

export namespace PluginQuery {
  export const getPlugins = (state: ApplicationState) => state.plugins.list;
  export const getLoaded = (state: ApplicationState) => state.plugins.isLoaded;
}
