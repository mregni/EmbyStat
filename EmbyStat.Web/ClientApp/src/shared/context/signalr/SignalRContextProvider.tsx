import React, {ReactElement} from 'react';

type Props = {
  children: ReactElement | ReactElement[];
}

export function SignalRContextProvider(props: Props) {
  const {children} = props;

  return ({children});
}
