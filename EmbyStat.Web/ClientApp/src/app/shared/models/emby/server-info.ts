import { Drive } from './drive';

export class ServerInfo {
  systemUpdateLevel: string;
  operatingSystemDisplayName: string;
  hasPendingRestart: boolean;
  isShuttingDown: boolean;
  supportsLibraryMonitor: boolean;
  webSocketPortNumber: number;
  canSelfRestart: boolean;
  canSelfUpdate: boolean;
  canLaunchWebBrowser: boolean;
  programDataPath: string;
  itemsByNamePath: string;
  cachePath: string;
  logPath: string;
  internalMetadataPath: string;
  transcodingTempPath: string;
  httpServerPortNumber: number;
  supportsHttps: boolean;
  httpsPortNumber: number;
  hasUpdateAvailable: boolean;
  supportsAutoRunAtStartup: boolean;
  encoderLocationType: string;
  systemArchitecture: string;
  localAddress: string;
  wanAddress: string;
  serverName: string;
  version: string;
  operatingSystem: string;
  drives: Drive[];
}
