import { Action } from '@ngrx/store';
import { Configuration } from '../../configuration/models/configuration';

export enum ConfigurationActionTypes {
  LOAD_CONFIGURATION = '[Configuration] Load Configuration',
  LOAD_CONFIGURATION_SUCCESS = '[Configuration] Load Configuration Success',
}

export class LoadConfigurationAction implements Action {
  readonly type = ConfigurationActionTypes.LOAD_CONFIGURATION;
  constructor(public payload = null) { }
}

export class LoadConfigurationSuccessAction implements Action {
  readonly type = ConfigurationActionTypes.LOAD_CONFIGURATION_SUCCESS;
  constructor(public payload: Configuration) { }
}

export type ConfigurationActions = LoadConfigurationAction | LoadConfigurationSuccessAction;
