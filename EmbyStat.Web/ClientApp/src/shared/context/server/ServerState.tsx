import {createContext, useState} from 'react';

import {MediaServerInfo, MediaServerPlugin} from '../../models/mediaServer';
import {getPlugins, getServerInfo} from '../../services';

export interface ServerContextProps {
  load: () => Promise<void>;
  loading: boolean;
  isLoaded: boolean;
  serverInfo: MediaServerInfo;
  plugins: MediaServerPlugin[];
}

export const ServerContext = createContext<ServerContextProps>(null!);

export const useServerContext = (): ServerContextProps => {
  const [loading, setLoading] = useState(false);
  const [isLoaded, setIsLoaded] = useState(false);
  const [serverInfo, setServerInfo] = useState<MediaServerInfo>(null!);
  const [plugins, setPlugins] = useState<MediaServerPlugin[]>(null!);

  const load = async (): Promise<void> => {
    if (!loading) {
      setLoading(true);
      const info = await getServerInfo();
      const plugins = await getPlugins();

      setServerInfo(info);
      setPlugins(plugins);
      setLoading(false);
      setIsLoaded(true);
    }
  };


  return {
    load, loading, serverInfo, plugins, isLoaded,
  };
};
