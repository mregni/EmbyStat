import { Settings } from '../models/settings';

const getFullMediaServerUrl = (settings: Settings): string => {
  const protocol =
    settings.mediaServer.serverProtocol === 0 ? 'https://' : 'http://';
  const baseUrl =
    settings.mediaServer.serverBaseUrl == null
      ? ''
      : settings.mediaServer.serverBaseUrl;

  return `${protocol}${settings.mediaServer.serverAddress}:${settings.mediaServer.serverPort}${baseUrl}`;
};

export default getFullMediaServerUrl;
