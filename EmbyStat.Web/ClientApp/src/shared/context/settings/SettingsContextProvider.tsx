import React, {ReactElement} from 'react';
import {useSettingsContext, SettingsContext} from '.';

interface Props {
  children: ReactElement | ReactElement[];
}

export const SettingsContextProvider = (props: Props): ReactElement => {
  const {children} = props;
  const settingsContext = useSettingsContext();

  return (
    <SettingsContext.Provider value={settingsContext}>
      {children}
    </SettingsContext.Provider>
  );
};
