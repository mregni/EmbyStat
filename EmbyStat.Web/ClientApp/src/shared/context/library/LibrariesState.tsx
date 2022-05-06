import {createContext, useState} from 'react';

import {Library} from '../../models/library';

export interface LibrariesContextProps {
  loading: boolean;
  loaded: boolean;
  load: (fetch: () => Promise<Library[]>) => Promise<void>;
  libraries: Library[];
  toggleLibrary: (id: string) => void;
  selected: string[];
  save: (push: (libraryIds: string[]) => Promise<void>) => Promise<void>;
}

export const LibrariesContext = createContext<LibrariesContextProps>(null!);

export const useLibrariesContext = (): LibrariesContextProps => {
  const [loading, setLoading] = useState(false);
  const [loaded, setLoaded] = useState(false);
  const [libraries, setLibraries] = useState<Library[]>([]);
  const [selected, setSelected] = useState<string[]>([]);

  const load = async (fetch: () => Promise<Library[]>): Promise<void> => {
    if (!loading && !loaded) {
      setLoading(true);
      const result = await fetch();
      setLibraries(result);
      setSelected(result.filter((x) => x.sync).map((x) => x.id));

      setLoading(false);
      setLoaded(true);
    }
  };

  const toggleLibrary = (id: string) => {
    const index = selected.indexOf(id);
    if (index !== -1) {
      setSelected((prev) => prev.filter((x) => x !== id));
    } else {
      setSelected((prev) => [...prev, id]);
    }
  };

  const save = async (push: (libraryIds: string[]) => Promise<void>): Promise<void> => {
    await push(selected);
  };

  return {
    load, loading, loaded, libraries,
    toggleLibrary, selected, save,
  };
};
