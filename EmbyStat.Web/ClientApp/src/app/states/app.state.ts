import { ActionReducerMap } from '@ngrx/store';
import { storeFreeze } from 'ngrx-store-freeze';

import { environment } from '../../environments/environment';

import { configurationReducer } from '../configuration/state/reducer.configuration';
import { Configuration } from '../configuration/models/configuration';

import { serverInfoReducer } from '../server/state/reducer.server';
import { ServerInfoStore } from '../server/models/server-info-store';

import { About } from '../about/models/about';
import { AboutReducer } from '../about/state/reducer.about';

export interface ApplicationState {
  configuration: Configuration;
  about: About;
  serverInfo: ServerInfoStore;
}

export const ROOT_REDUCER: ActionReducerMap<ApplicationState> = {
  configuration: configurationReducer,
  serverInfo: serverInfoReducer,
  about: AboutReducer
};
export const META_REDUCERS = !environment.production ? [storeFreeze] : [];
