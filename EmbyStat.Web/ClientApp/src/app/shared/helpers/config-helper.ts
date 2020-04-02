import { Settings } from '../models/settings/settings';

export class ConfigHelper {
  public static getFullEmbyAddress(settings: Settings): string {
    const protocol = settings.mediaServer.serverProtocol === 0 ? 'https://' : 'http://';
    return `${protocol}${settings.mediaServer.serverAddress}:${settings.mediaServer.serverPort}`;
  }
}
