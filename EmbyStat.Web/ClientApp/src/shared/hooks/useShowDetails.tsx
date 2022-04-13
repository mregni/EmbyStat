import {useState} from 'react';

import {Show} from '../models/common';
import {fetchShowDetails} from '../services/showService';

export const useShowDetails = () => {
  const [loading, setLoading] = useState<boolean>(false);
  const [show, setShow] = useState<Show>(null!);

  const getShowDetails = (id: string): void => {
    if (!loading) {
      setLoading(true);
      fetchShowDetails(id)
        .then((show) => setShow(show))
        .catch(() => setShow(null!))
        .finally(() => setLoading(false));
    }
  };

  return {loading, show, getShowDetails};
};
