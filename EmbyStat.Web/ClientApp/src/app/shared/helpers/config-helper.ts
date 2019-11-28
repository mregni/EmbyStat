import { Settings } from '../models/settings/settings';

export module ConfigHelper {
  export function getFullEmbyAddress(settings: Settings): string {
    const protocol = settings.emby.serverProtocol === 0 ? 'https://' : 'http://';
    return `${protocol}${settings.emby.serverAddress}:${settings.emby.serverPort}`;
  }
}
