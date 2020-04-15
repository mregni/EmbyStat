import { MediaServerSettings } from '../../../shared/models/settings/media-server-settings';
import { Settings } from '../../../shared/models/settings/settings';
import { TvdbSettings } from '../../../shared/models/settings/tvdb-settings';
import { ApplicationState } from '../../../states/app.state';
import { SettingsActions, SettingsActionTypes } from './settings.actions';

const mediaServerSettings: MediaServerSettings = {
  authorizationScheme: 'mediaBrowser',
  serverAddress: '',
  serverName: '',
  serverPort: 0,
  serverProtocol: 0,
  apiKey: '',
  serverType: 0,
  userId: ''
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
  mediaServer: mediaServerSettings,
  tvdb: tvdbSettings,
  enableRollbarLogging: false,
  isLoaded: false,
  toShortMovieEnabled: false,
  noUpdates: false,
  configDir: '',
  dataDir: '',
  logDir: ''
};

export function settingsReducer(state: Settings = INITIAL_STATE, action: SettingsActions) {
  switch (action.type) {
  case SettingsActionTypes.LOAD_SETTINGS_SUCCESS:
    return {
      ...state,
      language: action.payload.language,
      wizardFinished: action.payload.wizardFinished,
      username: action.payload.username,
      mediaServer: action.payload.mediaServer,
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
      toShortMovieEnabled: action.payload.toShortMovieEnabled,
      noUpdates: action.payload.noUpdates,
      configDir: action.payload.configDir,
      dataDir: action.payload.dataDir,
      logDir: action.payload.logDir,
      appName: action.payload.appName
    };
  case SettingsActionTypes.UPDATE_SETTINGS_SUCCESS:
    return {
      ...state,
      language: action.payload.language,
      wizardFinished: action.payload.wizardFinished,
      username: action.payload.username,
      mediaServer: action.payload.mediaServer,
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
      toShortMovieEnabled: action.payload.toShortMovieEnabled,
      noUpdates: action.payload.noUpdates,
      configDir: action.payload.configDir,
      dataDir: action.payload.dataDir,
      logDir: action.payload.logDir,
      appName: action.payload.appName
    };
  default:
    return state;
  }
}

export class SettingsQuery {
  public static getSettings = (state: ApplicationState) => state.settings;
  public static getLoaded = (state: ApplicationState) => state.settings.isLoaded;
}
