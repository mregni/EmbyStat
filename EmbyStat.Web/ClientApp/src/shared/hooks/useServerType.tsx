import { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import { RootState } from '../../store/RootReducer';

export function useServerType() {
  const settings = useSelector((state: RootState) => state.settings);
  const [serverType, setServerType] = useState('Emby');
  useEffect(() => {
    setServerType(settings.mediaServer.serverType === 0 ? 'Emby' : 'Jellyfin');
  }, [settings.mediaServer.serverType]);

  return serverType;
}