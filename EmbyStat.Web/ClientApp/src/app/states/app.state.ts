import { ActionReducerMap } from '@ngrx/store';
import { storeFreeze } from 'ngrx-store-freeze';

import { environment } from '../../environments/environment';

import { configurationReducer } from '../configuration/state/configuration.reducer';
import { Configuration } from '../configuration/models/configuration';

export interface ApplicationState {
  configuration: Configuration
}

export const ROOT_REDUCER: ActionReducerMap<ApplicationState> = { configuration: configurationReducer };
export const META_REDUCERS = !environment.production ? [storeFreeze] : [];
