import { Action } from '@ngrx/store';
import { EmbyPlugin } from '../models/embyPlugin';

export enum PluginActionTypes {
  LOAD_PLUGINS = '[Plugin] Load Plugins',
  LOAD_PLUGINS_SUCCESS = '[Plugin] Load Plugins Success',
  NOT_NEEDED = '[Plugin] Not needed',
  RESET_LOADED_STATE = '[Plugin] Reset Loaded State'
}

export class LoadPluginAction implements Action {
  readonly type = PluginActionTypes.LOAD_PLUGINS;
  constructor(public payload = null) { }
}

export class LoadPluginSuccessAction implements Action {
  readonly type = PluginActionTypes.LOAD_PLUGINS_SUCCESS;
  constructor(public payload: EmbyPlugin[]) { }
}

export class NoNeedPluginAction implements Action {
  readonly type = PluginActionTypes.NOT_NEEDED;
}

export class ResetPluginLoadedState implements Action {
  readonly type = PluginActionTypes.RESET_LOADED_STATE;
}


export type PluginActions = LoadPluginAction | LoadPluginSuccessAction |
                            NoNeedPluginAction | ResetPluginLoadedState;
