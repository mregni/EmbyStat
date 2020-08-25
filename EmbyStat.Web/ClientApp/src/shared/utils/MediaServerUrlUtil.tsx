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

export const getItemDetailLink = (settings: Settings, itemId: string): string => {
  if (settings.mediaServer.serverType === 0) {
    return `${getFullMediaServerUrl(settings)}/web/index.html#!/item?id=${itemId}&serverId=${settings.mediaServer.serverId}`;
  }

  return `${getFullMediaServerUrl(settings)}/web/index.html#!/details?id=${itemId}&serverId=${settings.mediaServer.serverId}`;
}

export const getPrimaryImageLink = (settings: Settings, itemId: string, tag: string): string => {
  return `${getFullMediaServerUrl(settings)}/Items/${itemId}/Images/Primary?maxHeight=350&tag=${tag}&quality=90&enableimageenhancers=false`;
}

export const getBackdropImageLink = (settings: Settings, itemId: string): string => {
  if (settings.mediaServer.serverType === 0) {
    return `${getFullMediaServerUrl(settings)}/Items/${itemId}/Images/Backdrop?quality=90&enableimageenhancers=false`;
  }

  return `${getFullMediaServerUrl(settings)}/Items/${itemId}/Images/Backdrop?quality=90`;
}

export default getFullMediaServerUrl;
