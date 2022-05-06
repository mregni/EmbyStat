import {useCallback, useEffect, useState} from 'react';

import {About} from '../../../shared/models/about';
import {getAbout} from '../../../shared/services';

export const useAbout = () => {
  const [loaded, setLoaded] = useState(false);
  const [about, setAbout] = useState<About>(null!);

  const load = useCallback(async () => {
    if (!loaded) {
      const result = await getAbout();
      setAbout(result);
      setLoaded(true);
    }
  }, [loaded]);

  useEffect(() => {
    (async () => {
      await load();
    })();
  }, [load]);

  return {about, loaded};
};
