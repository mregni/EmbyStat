import React, { ReactElement, useEffect, useState } from "react";
import { makeStyles } from '@material-ui/core/styles';
import { useTranslation } from "react-i18next";
import { StylesProvider, ThemeProvider } from "@material-ui/core/styles";
import AppBar from '@material-ui/core/AppBar';
import Toolbar from '@material-ui/core/Toolbar';
import CssBaseline from '@material-ui/core/CssBaseline';
import Typography from '@material-ui/core/Typography';
import { Switch, Route, useLocation, useHistory } from 'react-router-dom';
import { useDispatch, useSelector, useStore } from "react-redux";
import classNames from 'classnames';
import MenuIcon from '@material-ui/icons/Menu';
import ArrowBackRoundedIcon from '@material-ui/icons/ArrowBackRounded';
import useMediaQuery from '@material-ui/core/useMediaQuery';
import moment from 'moment';
import { SnackbarProvider } from 'notistack';

import 'devextreme/dist/css/dx.common.css';
import './styles/theme/dx.material.blue-yellow.css';

import { loadSettings } from './store/SettingsSlice';
import Menu from './shared/components/menu';
import Home from "./pages/home";
import MoviesLoader from "./pages/movies/Helpers/MoviesLoader";
import MoviesGeneral from "./pages/movies/MoviesGeneral";
import MoviesPeople from "./pages/movies/MoviesPeople";
import MoviesGraphs from "./pages/movies/MoviesGraphs";
import MoviesList from "./pages/movies/MoviesList";
import Wizard from "./pages/wizard";
import NotFound from "./pages/notFound";
import { RootState } from "./store/RootReducer";
import { SnackbarUtilsConfigurator } from './shared/utils/SnackbarUtilsConfigurator';

import theme from "./styles/theme";
import Jobs from "./pages/jobs";
import { IconButton } from "@material-ui/core";
import i18next from "i18next";
import { loadJobs } from "./store/JobSlice";
import SignalRConnectionProvider from "./store/SignalRConnectionProvider";


const useStyles = makeStyles((theme) => ({
  root: {
    display: 'flex',
    height: '100vh',
  },
  appBar: {
    zIndex: theme.zIndex.drawer + 1,
    transition: theme.transitions.create(['width', 'margin'], {
      easing: theme.transitions.easing.sharp,
      duration: theme.transitions.duration.leavingScreen,
    })
  },
  menuButton: {
    marginRight: theme.spacing(3),
    [theme.breakpoints.up('sm')]: {
      marginRight: theme.spacing(4),
    },
  },
  content: {
    padding: theme.spacing(3),
    marginTop: 56,
    [theme.breakpoints.up('sm')]: {
      marginTop: 64,
    },
    width: '100%',
    zIndex: 1,
  },
}));

function App(): ReactElement {
  const dispatch = useDispatch();
  const classes = useStyles();
  const { t } = useTranslation();
  const location = useLocation();
  const history = useHistory();
  const store = useStore();
  const [open, setOpen] = useState(false);

  const small = useMediaQuery(theme.breakpoints.down('md'));

  useEffect(() => {
    setOpen(!small);
  }, [small]);

  useEffect(() => {
    dispatch(loadSettings());
    dispatch(loadJobs());
  }, [dispatch]);

  const settings = useSelector((state: RootState) => state.settings);
  useEffect(() => {
    if (!settings.wizardFinished && location.pathname !== '/wizard') {
      history.replace('/wizard');
    } else if (settings.wizardFinished && location.pathname === '/wizard') {
      history.replace('/');
    }

    i18next.changeLanguage(settings.language);
    moment.locale(settings.language);
  }, [settings, location, history]);

  const handleDrawerToggle = () => {
    setOpen(!open);
  };

  return (
    <ThemeProvider theme={theme}>
      <StylesProvider injectFirst>
        <SignalRConnectionProvider store={store}>
          <SnackbarProvider
            maxSnack={3}
            anchorOrigin={{
              vertical: 'bottom',
              horizontal: 'right',
            }}>
            <SnackbarUtilsConfigurator />
            {settings.isLoaded ?
              <div className={classes.root}>
                <CssBaseline />
                <AppBar position="fixed" className={classNames(classes.appBar)}>
                  <Toolbar>
                    <IconButton
                      color="inherit"
                      aria-label="open drawer"
                      onClick={handleDrawerToggle}
                      edge="start"
                      className={classNames(classes.menuButton)}
                    >
                      {open ? <ArrowBackRoundedIcon /> : <MenuIcon />}
                    </IconButton>
                    <Typography variant="h6" noWrap>
                      {settings.appName}
                    </Typography>
                  </Toolbar>
                </AppBar>
                {settings.wizardFinished ? <Menu open={open} setOpen={setOpen} /> : null}

                <main className={classes.content}>
                  <Switch location={location}>
                    <Route path="/" exact>
                      <Home />
                    </Route>
                    <Route path="/movies" exact>
                      <MoviesLoader Component={MoviesGeneral} />
                    </Route>
                    <Route path="/movies/general" exact>
                      <MoviesLoader Component={MoviesGeneral} />
                    </Route>
                    <Route path="/movies/people" exact>
                      <MoviesLoader Component={MoviesPeople} />
                    </Route>
                    <Route path="/movies/graphs" exact>
                      <MoviesLoader Component={MoviesGraphs} />
                    </Route>
                    <Route path="/movies/list" exact>
                      <MoviesLoader Component={MoviesList} />
                    </Route>
                    <Route path="/wizard" exact>
                      <Wizard />
                    </Route>
                    <Route path="/jobs" exact>
                      <Jobs />
                    </Route>
                    <Route path="*">
                      <NotFound />
                    </Route>
                  </Switch>
                </main>
              </div> : "Loading webpage"}
          </SnackbarProvider>
        </SignalRConnectionProvider>
      </StylesProvider>
    </ThemeProvider >
  );
}

export default App;
