import {useContext, useEffect, useState} from 'react';

import {SettingsContext} from '../context/settings';

export function useServerType() {
  const {userConfig} = useContext(SettingsContext);
  const [serverType, setServerType] = useState('Emby');
  useEffect(() => {
    setServerType(userConfig?.mediaServer.type === 0 ? 'Emby' : 'Jellyfin');
  }, [userConfig?.mediaServer.type]);

  const getMediaServerTypeString = (): string => {
    if (userConfig != null) {
      return userConfig.mediaServer.type === 0 ? 'Emby' : 'Jellyfin';
    }
    return 'Emby';
  };

  const getMediaServerTypeStringFromNumber = (serverType: 0 | 1): string => {
    return serverType === 0 ? 'Emby' : 'Jellyfin';
  };

  return {serverType, getMediaServerTypeString, getMediaServerTypeStringFromNumber};
}
