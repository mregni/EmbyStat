import { Drive } from './drive';

export class ServerInfo {
  public supportsAutoRunAtStartup: boolean;
  public hasUpdateAvailable: boolean;
  public httpsPortNumber: number;
  public supportsHttps: boolean;
  public httpServerPortNumber: number;
  public transcodingTempPath: string;
  public internalMetadataPath: string;
  public logPath: string;
  public cachePath: string;
  public itemsByNamePath: string;
  public programDataPath: string;
  public encoderLocationType: string;
  public canSelfUpdate: boolean;
  public canSelfRestart: boolean;
  public webSocketPortNumber: number;
  public supportsLibraryMonitor: boolean;
  public isShuttingDown: boolean;
  public hasPendingRestart: boolean;
  public packageName: string;
  public operatingSystemDisplayName: string;
  public systemUpdateLevel: number;
  public canLaunchWebBrowser: boolean;
  public systemArchitecture: number;
  public drives: Drive[];
}
