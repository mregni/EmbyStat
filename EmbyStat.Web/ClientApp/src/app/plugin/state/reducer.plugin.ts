import { createSelector } from '@ngrx/store';
import { ApplicationState } from "../../states/app.state";

import { EmbyPlugin } from '../models/embyPlugin';
import { PluginActionTypes, PluginActions } from './actions.plugin';


const INITIAL_STATE: EmbyPlugin[] = [];

export function pluginReducer(state: EmbyPlugin[] = INITIAL_STATE, action: PluginActions) {
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
