import {createContext, useEffect, useState} from 'react';

import {MediaServerUserStatistics} from '../../models/mediaServer';
import {getUserStatistics} from '../../services';

export interface MediaServerUserProps {
  statistics: MediaServerUserStatistics;
  load: () => Promise<void>;
  loaded: boolean;
};

export const MediaServerUserContext = createContext<MediaServerUserProps>(null!);

export const useMediaServerUserContext = (): MediaServerUserProps => {
  const [loaded, setLoaded] = useState(false);
  const [statistics, setStatistics] = useState<MediaServerUserStatistics>(null!);

  const load = async (): Promise<void> => {
    const stats = await getUserStatistics();
    setStatistics(stats);
  };

  useEffect(() => {
    (async () => {
      if (!loaded) {
        await load();
        setLoaded(true);
      }
    })();
  }, []);

  return {load, loaded, statistics};
};
