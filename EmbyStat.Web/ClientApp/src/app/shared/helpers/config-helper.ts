import { Settings } from '../models/settings/settings';

export module ConfigHelper {
  export function getFullEmbyAddress(settings: Settings): string {
    const protocol = settings.mediaServer.serverProtocol === 0 ? 'https://' : 'http://';
    return `${protocol}${settings.mediaServer.serverAddress}:${settings.mediaServer.serverPort}`;
  }
}
