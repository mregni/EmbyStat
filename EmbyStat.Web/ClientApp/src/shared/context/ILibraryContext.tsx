import {ActiveFilter} from '../models/filter';

export interface LibraryContextProps<T> {
  statistics: T;
  activeFilters: ActiveFilter[];
  addFilter: (filter: ActiveFilter) => void;
  removeFilter: (id: string) => void;
  load: () => Promise<void>;
  loaded: boolean;
  mediaPresent: boolean;
}
