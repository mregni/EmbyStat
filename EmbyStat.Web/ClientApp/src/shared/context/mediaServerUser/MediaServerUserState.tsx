import {createContext, useState} from 'react';

import {MediaServerUser} from '../../models/mediaServer';
import {getUsers} from '../../services';

export interface MediaServerUserProps {
  load: () => Promise<void>;
  isLoaded: boolean;
  users: MediaServerUser[];
};

export const MediaServerUserContext = createContext<MediaServerUserProps>(null!);

export const useMediaServerUserContext = (): MediaServerUserProps => {
  const [loading, setLoading] = useState(false);
  const [isLoaded, setIsLoaded] = useState(false);
  const [users, setUsers] = useState<MediaServerUser[]>([]);

  const load = async (): Promise<void> => {
    if (!loading) {
      setLoading(true);

      const result = await getUsers();
      setUsers(result);
      setLoading(false);
      setIsLoaded(true);
    }
  };

  return {
    load, isLoaded, users,
  };
};
