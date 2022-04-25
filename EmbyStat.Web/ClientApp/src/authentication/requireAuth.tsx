import React, {ReactElement, useContext, useEffect, useState} from 'react';
import {Navigate} from 'react-router';
import {Box} from '@mui/material';

import {EsAppBar} from '../shared/components/esAppBar';
import {UserContext} from '../shared/context/user';
import {EsPageLoader} from '../shared/components/esPageLoader';
import {useEsLocation} from '../shared/hooks';
import {EsMenuDrawer} from '../shared/components/esMenuDrawer';

interface Props {
  children: ReactElement | ReactElement[];
}

export function RequireAuth(props: Props): ReactElement {
  const {children} = props;
  const location = useEsLocation();
  const userContext = useContext(UserContext);
  const [loading, setLoading] = useState(true);
  const [isLoggedIn, setIsLoggedIn] = useState(false);

  useEffect(() => {
    (async () => {
      const result = await userContext.isUserLoggedIn();
      setIsLoggedIn(result);
      setLoading(false);
    })();
  }, []);

  if (loading) {
    return <EsPageLoader />;
  }

  if (!isLoggedIn) {
    return <Navigate to="/login" state={{from: location}} replace={true} />;
  }

  return (
    <Box sx={{display: 'flex'}}>
      <EsAppBar />
      <EsMenuDrawer />
      <Box component="main" sx={{
        flexGrow: 1,
        bgcolor: 'background.default',
        pb: 3,
        pl: 3,
        pr: 3,
        pt: 11,
        minHeight: '100vh'}}>
        {children}
      </Box>
    </Box>);
}
