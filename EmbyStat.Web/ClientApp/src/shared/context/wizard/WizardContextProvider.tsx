import React, {ReactElement} from 'react';

import {useWizardContext, WizardContext} from './WizardState';

interface Props {
  children: ReactElement | ReactElement[];
}

export const WizardContextProvider = (props: Props): ReactElement => {
  const {children} = props;
  const wizardContext = useWizardContext();

  return (
    <WizardContext.Provider value={wizardContext}>
      {children}
    </WizardContext.Provider>
  );
};
