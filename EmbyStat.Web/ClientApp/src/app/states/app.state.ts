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

import { ShowStore } from '../show/models/showStore';
import { ShowReducer } from '../show/state/reducer.show';

import { LoadingState } from '../shared/components/loader/model/loadingState';
import { LoadingReducer } from '../shared/components/loader/state/reducer.loader';

import { About } from '../about/models/about';
import { AboutReducer } from '../about/state/reducer.about';

export interface ApplicationState {
  configuration: Configuration;
  about: About;
  plugins: EmbyPluginStore;
  serverInfo: ServerInfoStore;
  movies: MovieStore;
  shows: ShowStore;
  loading: LoadingState;
}

export const ROOT_REDUCER: ActionReducerMap<ApplicationState> = {
  configuration: configurationReducer,
  plugins: pluginReducer,
  serverInfo: serverInfoReducer,
  movies: MovieReducer,
  shows: ShowReducer,
  loading: LoadingReducer,
  about: AboutReducer
};
export const META_REDUCERS = !environment.production ? [storeFreeze] : [];
