export interface MediaServerSettings {
  serverName: string;
  apiKey: string;
  serverAddress: string;
  serverPort: number;
  authorizationScheme: string;
  serverProtocol: number;
  serverType: number;
  userId: string;
  serverBaseUrl: string;
  serverId: string;
}

export interface TmdbSettings {
  apiKey: string;
  lastUpdate: Date | null;
}

export interface Settings {
  id: string;
  appName: string;
  wizardFinished: boolean;
  language: string;
  toShortMovie: number;
  toShortMovieEnabled: boolean;
  keepLogsCount: number;
  movieLibraries: string[];
  showLibraries: string[];
  autoUpdate: boolean;
  updateTrain: number;
  updateInProgress: boolean;
  version: string;
  mediaServer: MediaServerSettings;
  tmdb: TmdbSettings;
  enableRollbarLogging: boolean;
  noUpdates: boolean;
  dataDir: string;
  logDir: string;
  configDir: string;
  isLoaded: boolean;
}

export const initialTmdbSettings: TmdbSettings = {
  apiKey: '',
  lastUpdate: null
}

export const intialMediaServerSettings: MediaServerSettings = {
  serverAddress: "",
  serverName: "",
  serverPort: 8096,
  serverProtocol: 0,
  apiKey: "",
  serverType: 0,
  authorizationScheme: '',
  serverBaseUrl: '',
  serverId: '',
  userId: ''
}

export const initialSettingsState: Settings = {
  appName: '',
  autoUpdate: true,
  configDir: '',
  dataDir: '',
  enableRollbarLogging: false,
  id: '',
  keepLogsCount: 10,
  language: 'en',
  logDir: '',
  movieLibraries: [],
  showLibraries: [],
  noUpdates: false,
  toShortMovie: 0,
  toShortMovieEnabled: false,
  updateInProgress: false,
  updateTrain: 0,
  version: '',
  wizardFinished: false,
  tmdb: initialTmdbSettings,
  mediaServer: intialMediaServerSettings,
  isLoaded: false
}

