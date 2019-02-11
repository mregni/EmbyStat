import { ApplicationState } from '../../states/app.state';

import { Settings } from '../models/settings';
import {
  SettingsActionTypes,
  SettingsActions
} from './actions.settings';
import { EmbySettings } from "../models/emby-settings";
import { TvdbSettings } from "../models/tvdb-settings";

const embySettings: EmbySettings = {
  accessToken: '',
  authorizationScheme: 'MediaBrowser',
  serverAddress: '',
  serverName: '',
  serverPort: 0,
  serverProtocol: 0,
  userId: '',
  userName: ''
}

const tvdbSettings: TvdbSettings = {
  apiKey: '',
  lastUpdate: undefined
}

const INITIAL_STATE: Settings = {
  id: '',
  appName: 'EmbyStat',
  wizardFinished: true,
  username: '',
  language: 'en-US',
  toShortMovie: 10,
  keepLogsCount: 20,
  movieCollectionTypes: [],
  showCollectionTypes: [],
  autoUpdate: false,
  updateTrain: 2,
  updateInProgress: false,
  version: "0.0.0.0",
  emby: embySettings,
  tvdb: tvdbSettings,
  isLoaded: false
};

export function settingsReducer(state: Settings = INITIAL_STATE, action: SettingsActions) {
  switch (action.type) {
    case SettingsActionTypes.LOAD_SETTINGS_SUCCESS:
      return {
        ...state,
        language: action.payload.language,
        wizardFinished: action.payload.wizardFinished,
        username: action.payload.username,
        emby: action.payload.emby,
        userId: action.payload.emby.userId,
        toShortMovie: action.payload.toShortMovie,
        id: action.payload.id,
        movieCollectionTypes: action.payload.movieCollectionTypes,
        showCollectionTypes: action.payload.showCollectionTypes,
        tvdb: action.payload.tvdb,
        keepLogsCount: action.payload.keepLogsCount,
        isLoaded: true,
        autoUpdate: action.payload.autoUpdate,
        updateTrain: action.payload.updateTrain,
        updateInProgress: action.payload.updateInProgress,
        version: action.payload.version
      };
    case SettingsActionTypes.UPDATE_SETTINGS_SUCCESS:
      return {
        ...state,
        language: action.payload.language,
        wizardFinished: action.payload.wizardFinished,
        username: action.payload.username,
        emby: action.payload.emby,
        userId: action.payload.emby.userId,
        toShortMovie: action.payload.toShortMovie,
        id: action.payload.id,
        movieCollectionTypes: action.payload.movieCollectionTypes,
        showCollectionTypes: action.payload.showCollectionTypes,
        tvdb: action.payload.tvdb,
        keepLogsCount: action.payload.keepLogsCount,
        isLoaded: true,
        autoUpdate: action.payload.autoUpdate,
        updateTrain: action.payload.updateTrain,
        updateInProgress: action.payload.updateInProgress,
        version: action.payload.version
      };
    default:
      return state;
  }
}

export namespace SettingsQuery {
  export const getSettings = (state: ApplicationState) => state.settings;
  export const getLoaded = (state: ApplicationState) => state.settings.isLoaded;
}
