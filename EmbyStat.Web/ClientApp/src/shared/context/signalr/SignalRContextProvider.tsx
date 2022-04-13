import React, {ReactElement} from 'react';

type Props = {
  children: ReactElement | ReactElement[];
}

export const SignalRContextProvider = (props: Props) => {
  const {children} = props;

  return (
    <>{children}</>
  );
};
