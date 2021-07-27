import { Settings } from '../models/settings';

export const getMediaServerTypeString = (settings: Settings): string => {
  return settings.mediaServer.serverType === 0 ? "Emby" : "Jellyfin";
};

export const getMediaServerTypeStringFromNumber = (serverType: 0 | 1): string => {
  return serverType === 0 ? "Emby" : "Jellyfin";
};
