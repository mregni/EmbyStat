import { Settings } from '../models/settings';

const getMediaServerTypeString = (settings: Settings): string => {
  return settings.mediaServer.serverType === 0 ? "Emby" : "Jellyfin";
};

export default getMediaServerTypeString;
