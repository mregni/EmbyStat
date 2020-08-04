import { Settings } from '../models/settings';

const getFullMediaServerUrl = (settings: Settings): string => {
  const protocol =
    settings.mediaServer.serverProtocol === 0 ? 'https://' : 'http://';
  const baseUrl =
    settings.mediaServer.serverBaseurl == null
      ? ''
      : settings.mediaServer.serverBaseurl;
  return `${protocol}${settings.mediaServer.serverAddress}:${settings.mediaServer.serverPort}${baseUrl}`;
};

export default getFullMediaServerUrl;
