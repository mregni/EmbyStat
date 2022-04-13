import {createContext, useEffect, useState} from 'react';

import {ActiveFilter} from '../../models/filter';
import {MovieStatistics} from '../../models/movie';
import {areMoviesPresent, getStatistics} from '../../services/movieService';
import {LibraryContextProps} from '../ILibraryContext';

export const MoviesContext = createContext<LibraryContextProps<MovieStatistics>>(null!);

export const useMoviesContext = (): LibraryContextProps<MovieStatistics> => {
  const [loaded, setLoaded] = useState(false);
  const [statistics, setStatistics] = useState<MovieStatistics>(null!);
  const [activeFilters, setActiveFilters] = useState<ActiveFilter[]>([]);
  const [mediaPresent, setMediaPresent] = useState(false);

  const load = async (): Promise<void> => {
    const stats = await getStatistics();
    setStatistics(stats);
    const present = await areMoviesPresent();
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
