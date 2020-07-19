import React, { ReactElement, useEffect } from 'react';
import { StylesProvider, ThemeProvider } from '@material-ui/core/styles';
import { useDispatch, useSelector, useStore } from 'react-redux';
import i18next from 'i18next';
import moment from 'moment';
import { SnackbarProvider } from 'notistack';
import theme from './styles/theme';
import CssBaseline from '@material-ui/core/CssBaseline';

import { loadSettings } from './store/SettingsSlice';
import { RootState } from './store/RootReducer';
import { SnackbarUtilsConfigurator } from './shared/utils/SnackbarUtilsConfigurator';
import LoggedIn from './container/LoggedIn';
import { loadJobs } from './store/JobSlice';
import SignalRConnectionProvider from './store/SignalRConnectionProvider';
import Wizard from './pages/wizard';

import 'devextreme/dist/css/dx.common.css';
import './styles/theme/dx.material.blue-yellow.css';
import PageLoader from './shared/components/pageLoader';

function App(): ReactElement {
  const dispatch = useDispatch();
  const store = useStore();

  useEffect(() => {
    dispatch(loadSettings());
    dispatch(loadJobs());
  }, [dispatch]);

  const settings = useSelector((state: RootState) => state.settings);
  useEffect(() => {
    i18next.changeLanguage(settings.language);
    moment.locale(settings.language);
  }, [settings]);

  console.log(settings.wizardFinished);


  return (
    <ThemeProvider theme={theme}>
      <StylesProvider injectFirst>
        <SignalRConnectionProvider store={store}>
          <CssBaseline />
          <SnackbarProvider
            maxSnack={3}
            anchorOrigin={{
              vertical: 'bottom',
              horizontal: 'right',
            }}
          >
            <SnackbarUtilsConfigurator />
            {settings.isLoaded && !settings.wizardFinished ? <Wizard settings={settings} /> : null}
            {settings.isLoaded && settings.wizardFinished ? <LoggedIn /> : null}
            {!settings.isLoaded ? <PageLoader /> : null}
          </SnackbarProvider>
        </SignalRConnectionProvider>
      </StylesProvider>
    </ThemeProvider>
  );
}

export default App;
