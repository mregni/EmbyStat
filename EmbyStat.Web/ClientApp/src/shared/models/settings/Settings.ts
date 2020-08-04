export interface MediaServerSettings {
  serverName: string;
  apiKey: string;
  serverAddress: string;
  serverPort: number;
  authorizationScheme: string;
  serverProtocol: number;
  serverType: number;
  userId: string;
  serverBaseurl: string;
  serverId: string;
}

export interface TvdbSettings {
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
  tvdb: TvdbSettings;
  enableRollbarLogging: boolean;
  isLoaded: boolean;
  noUpdates: boolean;
  dataDir: string;
  logDir: string;
  configDir: string;
}
