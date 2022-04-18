import React, {ReactElement} from 'react';

import {MediaServerUserContext, useMediaServerUserContext} from './MediaServerUserState';

interface Props {
  children: ReactElement | ReactElement[];
}

export const MediaServerUserContextProvider = (props: Props): ReactElement => {
  const {children} = props;
  const context = useMediaServerUserContext();

  return (
    <MediaServerUserContext.Provider value={context}>
      {children}
    </MediaServerUserContext.Provider>
  );
};
