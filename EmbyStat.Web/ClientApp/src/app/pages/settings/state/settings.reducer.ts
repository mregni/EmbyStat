import { EmbySettings } from '../../../shared/models/settings/emby-settings';
import { Settings } from '../../../shared/models/settings/settings';
import { TvdbSettings } from '../../../shared/models/settings/tvdb-settings';
import { ApplicationState } from '../../../states/app.state';
import { SettingsActions, SettingsActionTypes } from './settings.actions';

const embySettings: EmbySettings = {
  accessToken: '',
  authorizationScheme: 'MediaBrowser',
  serverAddress: '',
  serverName: '',
  serverPort: 0,
  serverProtocol: 0,
  userId: '',
  userName: ''
};

const tvdbSettings: TvdbSettings = {
  apiKey: '',
  lastUpdate: undefined
};

const INITIAL_STATE: Settings = {
  id: '',
  appName: 'EmbyStat',
  wizardFinished: true,
  username: '',
  language: 'en-US',
  toShortMovie: 10,
  keepLogsCount: 20,
  movieLibraryTypes: [],
  showLibraryTypes: [],
  autoUpdate: false,
  updateTrain: 2,
  updateInProgress: false,
  version: '0.0.0',
  emby: embySettings,
  tvdb: tvdbSettings,
  enableRollbarLogging: false,
  isLoaded: false,
  toShortMovieEnabled: false
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
        movieLibraryTypes: action.payload.movieLibraryTypes,
        showLibraryTypes: action.payload.showLibraryTypes,
        tvdb: action.payload.tvdb,
        keepLogsCount: action.payload.keepLogsCount,
        isLoaded: true,
        autoUpdate: action.payload.autoUpdate,
        updateTrain: action.payload.updateTrain,
        updateInProgress: action.payload.updateInProgress,
        version: action.payload.version,
        enableRollbarLogging: action.payload.enableRollbarLogging,
        toShortMovieEnabled: action.payload.toShortMovieEnabled
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
        movieLibraryTypes: action.payload.movieLibraryTypes,
        showLibraryTypes: action.payload.showLibraryTypes,
        tvdb: action.payload.tvdb,
        keepLogsCount: action.payload.keepLogsCount,
        isLoaded: true,
        autoUpdate: action.payload.autoUpdate,
        updateTrain: action.payload.updateTrain,
        updateInProgress: action.payload.updateInProgress,
        version: action.payload.version,
        enableRollbarLogging: action.payload.enableRollbarLogging,
        toShortMovieEnabled: action.payload.toShortMovieEnabled
      };
    default:
      return state;
  }
}

export namespace SettingsQuery {
  export const getSettings = (state: ApplicationState) => state.settings;
  export const getLoaded = (state: ApplicationState) => state.settings.isLoaded;
}
