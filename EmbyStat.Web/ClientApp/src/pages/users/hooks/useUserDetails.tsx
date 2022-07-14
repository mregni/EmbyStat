import {useState} from 'react';

import {MediaServerUserDetails} from '../../../shared/models/mediaServer';
import {getUserDetails} from '../../../shared/services';

export const useUserDetails = () => {
  const [loading, setLoading] = useState(true);
  const [userDetails, setUserDetails] = useState<MediaServerUserDetails>(null!);

  const fetchUserDetails = async (id: string | undefined) => {
    if (id !== undefined) {
      setLoading(true);
      const user = await getUserDetails(id);
      setUserDetails(user);
      setLoading(false);
    }
  };

  return {
    loading, userDetails, fetchUserDetails,
  };
};
