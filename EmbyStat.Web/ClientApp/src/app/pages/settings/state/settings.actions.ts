import { Action } from '@ngrx/store';

import { Settings } from '../../../shared/models/settings/settings';

export enum SettingsActionTypes {
  LOAD_SETTINGS = '[Settings] Load Settings',
  LOAD_SETTINGS_SUCCESS = '[Settings] Load Settings Success',
  UPDATE_SETTINGS = '[Settings] Update Settings',
  UPDATE_SETTINGS_SUCCESS = '[Settings] Updated Settings Success',
  NOT_NEEDED = '[Settings] Not Needed'
}

export class LoadSettingsAction implements Action {
  readonly type = SettingsActionTypes.LOAD_SETTINGS;
  constructor(public payload = null) { }
}

export class LoadSettingsSuccessAction implements Action {
  readonly type = SettingsActionTypes.LOAD_SETTINGS_SUCCESS;
  constructor(public payload: Settings) { }
}

export class UpdateSettingsAction implements Action {
  readonly type = SettingsActionTypes.UPDATE_SETTINGS;
  constructor(public payload: Settings) { }
}

export class UpdateSettingsSuccessAction implements Action {
  readonly type = SettingsActionTypes.UPDATE_SETTINGS_SUCCESS;
  constructor(public payload: Settings) { }
}

export class NoNeedSettingsAction implements Action {
  readonly type = SettingsActionTypes.NOT_NEEDED;
}

export type SettingsActions = LoadSettingsAction | LoadSettingsSuccessAction |
                                   UpdateSettingsAction | UpdateSettingsSuccessAction |
                                   NoNeedSettingsAction;
