import { storeFreeze } from 'ngrx-store-freeze';

import { ActionReducerMap } from '@ngrx/store';

import { environment } from '../../environments/environment';
import { settingsReducer } from '../pages/settings/state/settings.reducer';
import { Settings } from '../shared/models/settings/settings';

export interface ApplicationState {
  settings: Settings;
}

export const ROOT_REDUCER: ActionReducerMap<ApplicationState> = {
  settings: settingsReducer
};
export const META_REDUCERS = !environment.production ? [storeFreeze] : [];
