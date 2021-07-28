import { useContext, useEffect, useState } from 'react';
import { SettingsContext } from '../context/settings';

export function useServerType() {
  const { settings } = useContext(SettingsContext);
  const [serverType, setServerType] = useState('Emby');
  useEffect(() => {
    setServerType(settings.mediaServer.serverType === 0 ? 'Emby' : 'Jellyfin');
  }, [settings.mediaServer.serverType]);

  return serverType;
}
