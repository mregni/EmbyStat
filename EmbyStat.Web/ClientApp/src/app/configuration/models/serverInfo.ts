export class ServerInfo {
  id: string;
  supportsAutoRunAtStartup: boolean;
  hasUpdateAvailable: boolean;
  httpsPortNumber: number;
  supportsHttps: boolean;
  httpServerPortNumber: number;
  transcodingTempPath: string;
  internalMetadataPath: string;
  logPath: string;
  cachePath: string;
  itemsByNamePath: string;
  programDataPath: string;
  encoderLocationType: string;
  canSelfUpdate: boolean;
  canSelfRestart: boolean;
  webSocketPortNumber: number;
  supportsLibraryMonitor: boolean;
  isShuttingDown: boolean;
  hasPendingRestart: boolean;
  packageName: string;
  operatingSystemDisplayName: string;
  systemUpdateLevel: number;
  canLaunchWebBrowser: boolean;
  systemArchitecture: number;
}
