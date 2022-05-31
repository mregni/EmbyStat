export interface Settings {
    userConfig: UserConfig;
    systemConfig: SystemConfig;
}

export interface UserConfig {
    language: string;
    toShortMovieEnabled: boolean;
    toShortMovie: number;
    keepLogsCount: number;
    logLevel: number;
    enableRollbarLogging: boolean;
    wizardFinished: boolean;
    hosting: Hosting;
    mediaServer: MediaServerSettings;
    tmdb: TmdbSettings;
}

export interface SystemConfig {
    autoUpdate: boolean;
    version: string;
    processName: string;
    appName: string;
    id: string | null;
    updateInProgress: boolean;
    updatesDisabled: boolean;
    migration: number;
    updateTrain: number;
    rollbar: Rollbar;
    dirs: Dirs;
}

export interface Hosting {
    sslEnabled: boolean;
    port: number;
    sslPort: number;
    url: string;
    sslCertPath: string;
    sslCertPassword: string;
}

export interface MediaServerSettings {
    name: string;
    apiKey: string;
    address: string;
    authorizationScheme: string;
    type: number;
    userId: string;
    id: string;
    fullSocketAddress: string;
}

export interface TmdbSettings {
    apiKey: string;
}

export interface Rollbar {
    enabled: boolean;
}

export interface Dirs {
    tempUpdate: string;
    updater: string;
    logs: string;
    data: string;
}


export interface LibraryContainer {
  id: string;
  lastSynced: Date | null;
  name: string;
}

