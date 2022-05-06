import React, {ReactElement} from 'react';

import {MoviesContext, useMoviesContext} from './MoviesState';

interface Props {
  children: ReactElement | ReactElement[];
}

export function MoviesContextProvider(props: Props): ReactElement {
  const {children} = props;
  const moviesContext = useMoviesContext();

  return (
    <MoviesContext.Provider value={moviesContext}>
      {children}
    </MoviesContext.Provider>
  );
}
