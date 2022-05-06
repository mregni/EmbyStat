import React, {ReactElement} from 'react';

import {ShowsContext, useShowsContext} from '.';

interface Props {
  children: ReactElement | ReactElement[];
}

export function ShowsContextProvider(props: Props): ReactElement {
  const {children} = props;
  const showsContext = useShowsContext();

  return (
    <ShowsContext.Provider value={showsContext}>
      {children}
    </ShowsContext.Provider>
  );
}
