import {createContext, useEffect, useState} from 'react';

import {ActiveFilter} from '../../models/filter';
import {ShowStatistics} from '../../models/show';
import {areShowsPresent, getStatistics} from '../../services/showService';
import {LibraryContextProps} from '../ILibraryContext';

export const ShowsContext = createContext<LibraryContextProps<ShowStatistics>>(null!);

export const useShowsContext = (): LibraryContextProps<ShowStatistics> => {
  const [loaded, setLoaded] = useState(false);
  const [statistics, setStatistics] = useState<ShowStatistics>(null!);
  const [activeFilters, setActiveFilters] = useState<ActiveFilter[]>([]);
  const [mediaPresent, setMediaPresent] = useState(false);


  const load = async (): Promise<void> => {
    const stats = await getStatistics();
    setStatistics(stats);
    const present = await areShowsPresent();
    setMediaPresent(present);
  };

  const addFilter = (filter: ActiveFilter) => {
    setActiveFilters((prev) => [...prev, filter]);
  };

  const removeFilter = (id: string) => {
    setActiveFilters((prev) => prev.filter((x) => x.id !== id));
  };

  useEffect(() => {
    (async () => {
      if (!loaded) {
        await load();
        setLoaded(true);
      }
    })();
  }, []);

  return {
    statistics, activeFilters,
    removeFilter, addFilter, load,
    loaded, mediaPresent,
  };
};
