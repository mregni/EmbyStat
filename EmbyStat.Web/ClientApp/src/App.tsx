import {SnackbarProvider} from 'notistack';
import React, {useContext, useEffect} from 'react';

import {CssBaseline, ThemeProvider} from '@mui/material';

import RoutesContainer from './routes';
import {EsPageLoader} from './shared/components/esPageLoader';
import {SettingsContext} from './shared/context/settings';
import {useSignalR} from './shared/context/signalr';
import {SnackbarUtilsConfigurator} from './shared/utils/SnackbarUtilsConfigurator';
import {theme} from './styles/theme';

function App() {
  const {settings, load} = useContext(SettingsContext);
  useSignalR();

  useEffect(() => {
    if (!settings?.isLoaded) {
      load();
    }
  }, [load, settings]);

  return (
    <ThemeProvider theme={theme}>
      <CssBaseline />
      <SnackbarProvider
        maxSnack={3}
        anchorOrigin={{
          vertical: 'bottom',
          horizontal: 'right',
        }}
      >
        {settings === null && <EsPageLoader />}
        {settings !== null && <RoutesContainer />}
        <SnackbarUtilsConfigurator />
      </SnackbarProvider>
    </ThemeProvider>
  );
}

export default App;
