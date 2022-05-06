import {useState} from 'react';

import {MediaServerUdpBroadcast} from '../models/mediaServer';
import {searchMediaServers} from '../services';

export const useSearchMediaServers = () => {
  const [servers, setServers] = useState<MediaServerUdpBroadcast[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [loaded, setLoaded] = useState(false);

  const fetchMediaServers = async (): Promise<void> => {
    try {
      setIsLoading(true);
      const result = await searchMediaServers();
      setServers(result);
    } catch (error) {
      setServers([]);
    } finally {
      setIsLoading(false);
      setLoaded(true);
    }
  };

  return {
    servers, isLoading, loaded, fetchMediaServers,
  };
};
