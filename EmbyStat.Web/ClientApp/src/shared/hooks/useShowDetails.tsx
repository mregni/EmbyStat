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
        .finally(() => setLoading(false))
        .catch(() => setShow(null!));
    }
  };

  return {loading, show, getShowDetails};
};
