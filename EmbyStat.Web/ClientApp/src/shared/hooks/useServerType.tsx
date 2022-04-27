import {useContext, useEffect, useState} from 'react';

import {SettingsContext} from '../context/settings';

export function useServerType() {
  const {settings} = useContext(SettingsContext);
  const [serverType, setServerType] = useState('Emby');
  useEffect(() => {
    setServerType(settings?.mediaServer.type === 0 ? 'Emby' : 'Jellyfin');
  }, [settings?.mediaServer.type]);

  const getMediaServerTypeString = (): string => {
    if (settings != null) {
      return settings.mediaServer.type === 0 ? 'Emby' : 'Jellyfin';
    }
    return 'Emby';
  };

  const getMediaServerTypeStringFromNumber = (serverType: 0 | 1): string => {
    return serverType === 0 ? 'Emby' : 'Jellyfin';
  };

  return {serverType, getMediaServerTypeString, getMediaServerTypeStringFromNumber};
}
