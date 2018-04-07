import { Action } from '@ngrx/store';
import { Configuration } from '../models/configuration';

export enum ConfigurationActionTypes {
  LOAD_CONFIGURATION = '[Configuration] Load Configuration',
  LOAD_CONFIGURATION_SUCCESS = '[Configuration] Load Configuration Success',
  UPDATE_CONFIGURATION = '[Configuration] Update Configuration',
  UPDATE_CONFIGURATION_SUCCESS = '[Configuration] Updated Configuration Success',
  FIRE_SMALL_EMBY_SYNC = '[Configuration] Start Small Emby Sync',
  NOT_NEEDED = '[Configuration] Not Needed'
}

export class LoadConfigurationAction implements Action {
  readonly type = ConfigurationActionTypes.LOAD_CONFIGURATION;
  constructor(public payload = null) { }
}

export class LoadConfigurationSuccessAction implements Action {
  readonly type = ConfigurationActionTypes.LOAD_CONFIGURATION_SUCCESS;
  constructor(public payload: Configuration) { }
}

export class UpdateConfigurationAction implements Action {
  readonly type = ConfigurationActionTypes.UPDATE_CONFIGURATION;
  constructor(public payload: Configuration) { }
}

export class UpdateConfigurationSuccessAction implements Action {
  readonly type = ConfigurationActionTypes.UPDATE_CONFIGURATION_SUCCESS;
  constructor(public payload: Configuration) { }
}

export class FireSmallEmbySyncAction implements Action {
  readonly type = ConfigurationActionTypes.FIRE_SMALL_EMBY_SYNC;
  constructor(public payload = false) { }
}

export class NoNeedConfigurationAction implements Action {
  readonly type = ConfigurationActionTypes.NOT_NEEDED;
}

export type ConfigurationActions = LoadConfigurationAction | LoadConfigurationSuccessAction |
  UpdateConfigurationAction | UpdateConfigurationSuccessAction |
  FireSmallEmbySyncAction | NoNeedConfigurationAction;
