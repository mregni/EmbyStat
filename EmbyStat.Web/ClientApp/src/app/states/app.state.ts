import { storeFreeze } from 'ngrx-store-freeze';

import { ActionReducerMap } from '@ngrx/store';

import { environment } from '../../environments/environment';
import { embyServerInfoReducer } from '../pages/server/state/emby-server.reducer';
import { settingsReducer } from '../pages/settings/state/settings.reducer';
import { ServerInfo } from '../shared/models/emby/server-info';
import { Settings } from '../shared/models/settings/settings';

export interface ApplicationState {
  settings: Settings;
  embyServerInfo: ServerInfo;
}

export const ROOT_REDUCER: ActionReducerMap<ApplicationState> = {
  settings: settingsReducer,
  embyServerInfo: embyServerInfoReducer
};
export const META_REDUCERS = !environment.production ? [storeFreeze] : [];
