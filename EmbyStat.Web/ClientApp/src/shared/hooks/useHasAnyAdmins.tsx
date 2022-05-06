import {useEffect, useState} from 'react';

import {anyAdmins} from '../services';

export const useHasAnyAdmins = () => {
  const [hasAdmins, setHasAdmins] = useState<boolean>(false);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    let didCancel = false;

    const fetchData = async () => {
      try {
        setIsLoading(true);
        const result = await anyAdmins();

        if (!didCancel) {
          setHasAdmins(result);
        }
      } catch (error) {
        if (!didCancel) {
          setHasAdmins(true);
        }
      } finally {
        setIsLoading(false);
      }
    };
    fetchData();
    return () => {
      didCancel = true;
    };
  }, []);

  return {
    hasAdmins, isLoading,
  };
};
