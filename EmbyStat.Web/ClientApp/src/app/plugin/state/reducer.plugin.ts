import { createSelector } from '@ngrx/store';
import { ApplicationState } from "../../states/app.state";

import { EmbyPluginStore } from '../models/embyPluginStore';
import { PluginActionTypes, PluginActions } from './actions.plugin';


const INITIAL_STATE: EmbyPluginStore = {
  list: []
};

export function pluginReducer(state: EmbyPluginStore = INITIAL_STATE, action: PluginActions) {
  switch (action.type) {
  case PluginActionTypes.LOAD_PLUGINS_SUCCESS:
    return {
      ...state,
      list: action.payload
    };
  default:
    return state;
  }
}

export namespace PluginQuery {
  export const getPlugins = (state: ApplicationState) => state.plugins;
}
