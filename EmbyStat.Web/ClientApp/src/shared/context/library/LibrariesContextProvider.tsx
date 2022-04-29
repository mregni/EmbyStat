import React, {ReactElement} from 'react';

import {LibrariesContext, useLibrariesContext} from '.';

interface Props {
  children: ReactElement | ReactElement[];
}

export function LibrariesContextProvider(props: Props): ReactElement {
  const {children} = props;
  const librariesContext = useLibrariesContext();

  return (
    <LibrariesContext.Provider value={librariesContext}>
      {children}
    </LibrariesContext.Provider>
  );
}
