export interface MediaServerSettings {
  name: string;
  apiKey: string;
  address: string;
  authorizationScheme: string;
  type: number;
  userId: string;
  id: string;
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

export interface LibraryContainer {
  id: string;
  lastSynced: Date | null;
  name: string;
}

