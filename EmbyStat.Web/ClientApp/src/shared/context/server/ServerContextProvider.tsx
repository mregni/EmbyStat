import React, {ReactElement} from 'react';

import {ServerContext, useServerContext} from './';

interface Props {
  children: ReactElement | ReactElement[];
}

export const ServerContextProvider = (props: Props): ReactElement => {
  const {children} = props;
  const serverContext = useServerContext();

  return (
    <ServerContext.Provider value={serverContext}>
      {children}
    </ServerContext.Provider>
  );
};
