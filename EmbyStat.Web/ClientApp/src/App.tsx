import React, { ReactElement, useContext, useEffect } from 'react';
import { StylesProvider, ThemeProvider } from '@material-ui/core/styles';
import { useDispatch, useStore } from 'react-redux';
import i18n from './i18n';
import moment from 'moment';
import { SnackbarProvider } from 'notistack';
import theme from './styles/theme';
import CssBaseline from '@material-ui/core/CssBaseline';
import { useHistory } from 'react-router';

import { SnackbarUtilsConfigurator } from './shared/utils/SnackbarUtilsConfigurator';
import LoggedIn from './container/LoggedIn';
import { loadJobs } from './store/JobSlice';
import SignalRConnectionProvider from './shared/providers/SignalRConnectionProvider';
import { WizardContainer } from './pages/wizard';
import PageLoader from './shared/components/pageLoader';
import { SettingsContext } from './shared/context/settings';

function App(): ReactElement {
  const dispatch = useDispatch();
  const history = useHistory();
  const store = useStore();
  const { settings, loadSettings } = useContext(SettingsContext);

  useEffect(() => {
    if (!settings.isLoaded) {
      loadSettings();
    } else {
      i18n.changeLanguage(settings.language);
      moment.locale(settings.language);
    }
  }, [loadSettings, settings]);

  useEffect(() => {
    dispatch(loadJobs());
  }, [dispatch]);

  useEffect(() => {
    if (settings.isLoaded && !settings.wizardFinished) {
      history.push('/wizard');
    }
  }, [history, settings.isLoaded, settings.wizardFinished])

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
            {!settings.isLoaded && <PageLoader />}
            {settings.isLoaded && !settings.wizardFinished && <WizardContainer />}
            {settings.isLoaded && settings.wizardFinished && <LoggedIn />}
          </SnackbarProvider>
        </SignalRConnectionProvider>
      </StylesProvider>
    </ThemeProvider>
  );
}

export default App;
