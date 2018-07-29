import { Drive } from './drive';

export class ServerInfo {
  public systemUpdateLevel: string;
  public operatingSystemDisplayName: string;
  public hasPendingRestart: boolean;
  public isShuttingDown: boolean;
  public supportsLibraryMonitor: boolean;
  public webSocketPortNumber: number;
  public canSelfRestart: boolean;
  public canSelfUpdate: boolean;
  public canLaunchWebBrowser: boolean;
  public programDataPath: string;
  public itemsByNamePath: string;
  public cachePath: string;
  public logPath: string;
  public internalMetadataPath: string;
  public transcodingTempPath: string;
  public httpServerPortNumber: number;
  public supportsHttps: boolean;
  public httpsPortNumber: number;
  public hasUpdateAvailable: boolean;
  public supportsAutoRunAtStartup: boolean;
  public encoderLocationType: string;
  public systemArchitecture: string;
  public localAddress: string;
  public wanAddress: string;
  public serverName: string;
  public version: string;
  public operatingSystem: string;
  public drives: Drive[];
}
