import { Action } from '@ngrx/store';
import { EmbyPlugin } from '../models/embyPlugin';

export enum PluginActionTypes {
  LOAD_PLUGINS = '[Plugin] Load Plugins',
  LOAD_PLUGINS_SUCCESS = '[Plugin] Load Plugins Success'
}

export class LoadPluginAction implements Action {
  readonly type = PluginActionTypes.LOAD_PLUGINS;
  constructor(public payload = null) { }
}

export class LoadPluginSuccessAction implements Action {
  readonly type = PluginActionTypes.LOAD_PLUGINS_SUCCESS;
  constructor(public payload: EmbyPlugin[]) { }
}

export type PluginActions = LoadPluginAction | LoadPluginSuccessAction;
