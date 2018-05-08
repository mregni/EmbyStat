import { ActionReducerMap } from '@ngrx/store';
import { storeFreeze } from 'ngrx-store-freeze';

import { environment } from '../../environments/environment';

import { configurationReducer } from '../configuration/state/reducer.configuration';
import { Configuration } from '../configuration/models/configuration';

import { pluginReducer } from '../plugin/state/reducer.plugin';
import { EmbyPluginStore } from '../plugin/models/embyPluginStore';

import { serverInfoReducer } from '../server/state/reducer.server';
import { ServerInfoStore } from '../server/models/serverInfoStore';

import { MovieStore } from '../movie/models/movieStore';
import { MovieReducer } from '../movie/state/reducer.movie';

export interface ApplicationState {
  configuration: Configuration;
  plugins: EmbyPluginStore;
  serverInfo: ServerInfoStore;
  movies: MovieStore;
}

export const ROOT_REDUCER: ActionReducerMap<ApplicationState> = {
    configuration: configurationReducer,
    plugins: pluginReducer,
    serverInfo: serverInfoReducer,
    movies: MovieReducer
};
export const META_REDUCERS = !environment.production ? [storeFreeze] : [];
