import { Settings } from '../models/settings/settings';

export module ConfigHelper {
  export function getFullEmbyAddress(settings: Settings): string {
    const protocol = settings.emby.serverProtocol === 0 ? 'http://' : 'https://';
    const url = protocol + settings.emby.serverAddress + ':' + settings.emby.serverPort;
    return url;
  }
}