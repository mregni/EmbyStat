import { ActionReducerMap } from '@ngrx/store';
import { storeFreeze } from 'ngrx-store-freeze';

import { environment } from '../../environments/environment';

import { settingsReducer } from '../settings/state/reducer.settings';
import { Settings } from '../settings/models/settings';

import { serverInfoReducer } from '../server/state/reducer.server';
import { ServerInfoStore } from '../server/models/server-info-store';

import { About } from '../about/models/about';
import { AboutReducer } from '../about/state/reducer.about';

export interface ApplicationState {
  settings: Settings;
  about: About;
  serverInfo: ServerInfoStore;
}

export const ROOT_REDUCER: ActionReducerMap<ApplicationState> = {
  settings: settingsReducer,
  serverInfo: serverInfoReducer,
  about: AboutReducer
};
export const META_REDUCERS = !environment.production ? [storeFreeze] : [];
