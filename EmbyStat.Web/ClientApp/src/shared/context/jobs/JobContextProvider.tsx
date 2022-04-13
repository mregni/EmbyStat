import React, {ReactElement} from 'react';
import {useJobsContext, JobsContext} from '.';

interface Props {
  children: ReactElement | ReactElement[];
}

export const JobsContextProvider = (props: Props): ReactElement => {
  const {children} = props;
  const jobsContext = useJobsContext();

  return (
    <JobsContext.Provider value={jobsContext}>
      {children}
    </JobsContext.Provider>
  );
};
