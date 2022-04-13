import React, {ReactElement} from 'react';

import {LibrariesContext, useLibrariesContext} from './';

interface Props {
  children: ReactElement | ReactElement[];
}

export const LibrariesContextProvider = (props: Props): ReactElement => {
  const {children} = props;
  const librariesContext = useLibrariesContext();

  return (
    <LibrariesContext.Provider value={librariesContext}>
      {children}
    </LibrariesContext.Provider>
  );
};
