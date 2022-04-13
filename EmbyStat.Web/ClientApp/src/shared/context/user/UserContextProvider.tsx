import React, {ReactElement} from 'react';
import {useUserContext, UserContext} from '.';

interface Props {
  children: ReactElement | ReactElement[];
}

export const UserContextProvider = (props: Props): ReactElement => {
  const {children} = props;
  const userContext = useUserContext();

  return (
    <UserContext.Provider value={userContext}>
      {children}
    </UserContext.Provider>
  );
};
