import { ActionReducerMap } from '@ngrx/store';
import { storeFreeze } from 'ngrx-store-freeze';

import { environment } from '../../environments/environment';

import { ConfigurationState, configurationReducer } from './configuration/configuration.reducer';

export interface ApplicationState {
  configuration: ConfigurationState
}

export const ROOT_REDUCER: ActionReducerMap<ApplicationState> = { configuration: configurationReducer };
export const META_REDUCERS = !environment.production ? [storeFreeze] : [];
