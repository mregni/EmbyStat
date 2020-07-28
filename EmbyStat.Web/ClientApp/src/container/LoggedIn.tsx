import React, { useEffect, useState } from 'react';
import classNames from 'classnames';
import { Switch, Route, useHistory, RouteComponentProps, withRouter } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { makeStyles } from '@material-ui/core/styles';
import MenuIcon from '@material-ui/icons/Menu';
import ArrowBackRoundedIcon from '@material-ui/icons/ArrowBackRounded';
import AppBar from '@material-ui/core/AppBar';
import Toolbar from '@material-ui/core/Toolbar';
import Typography from '@material-ui/core/Typography';
import useMediaQuery from '@material-ui/core/useMediaQuery';
import IconButton from '@material-ui/core/IconButton';
import Grid from '@material-ui/core/Grid';
import Button from '@material-ui/core/Button';
import CircularProgress from '@material-ui/core/CircularProgress';
import { StaticContext } from 'react-router';

import Menu from '../shared/components/menu';
import Home from '../pages/home';
import MoviesLoader from '../pages/movies/Helpers/MoviesLoader';
import MoviesGeneral from '../pages/movies/MoviesGeneral';
import MoviesPeople from '../pages/movies/MoviesPeople';
import MoviesGraphs from '../pages/movies/MoviesGraphs';
import MoviesList from '../pages/movies/MoviesList';
import Jobs from '../pages/jobs';
import NotFound from '../pages/notFound';
import Login from '../pages/login';
import GeneralSettings from '../pages/settings/GeneralSettings';
import MovieSettings from '../pages/settings/MovieSettings';
import PrivateRoute from '../shared/components/privateRoute';
import theme from '../styles/theme';
import { userLoggedIn$, logout } from '../shared/services/AccountService';

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
    }),
  },
  appBar__closed: {
    display: 'none',
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
  header__buttons: {
    width: 250,
  },
  button__loading: {
    color: '#d3d3d3'
  },
  logout__button: {
    width: 88,
    height: 36,
  }
}));

type Props = RouteComponentProps<
  {},
  StaticContext,
  { referer: { pathname: string } }
>;

const LoggedIn = (props: Props) => {
  const { location } = props;
  const classes = useStyles();
  const { t } = useTranslation();
  const history = useHistory();
  const [openMenu, setOpenMenu] = useState(false);
  const [openHeader, setOpenHeader] = useState(false);
  const [isLoading, setIsLoading] = useState(false);

  const small = useMediaQuery(theme.breakpoints.down('md'));

  useEffect(() => {
    setOpenMenu(!small);
  }, [small]);

  useEffect(() => {
    userLoggedIn$.subscribe((value: boolean) => {
      setOpenMenu(value);
      setOpenHeader(value);
    });
    return (() => {
      userLoggedIn$.unsubscribe();
    })
  }, []);

  const handleDrawerToggle = () => {
    setOpenMenu(!openMenu);
  };

  const logoutUser = () => {
    setIsLoading(true);
    logout().finally(() => {
      history.push('/login', { referer: { pathname: '' } });
      setIsLoading(false);
    });
  }

  return (
    <div className={classes.root}>
      <AppBar position="fixed" className={classNames(classes.appBar, { [classes.appBar__closed]: !openHeader })}>
        <Toolbar>
          <Grid container direction="row" alignItems="center" justify="space-between">
            <Grid item className={classes.header__buttons} container direction="row" alignItems="center" >
              <IconButton
                color="inherit"
                onClick={handleDrawerToggle}
                edge="start"
                className={classNames(classes.menuButton)}
              >
                {openMenu ? <ArrowBackRoundedIcon /> : <MenuIcon />}
              </IconButton>
              <Typography variant="h6" noWrap>
                EmbyStat
              </Typography>
            </Grid>
            <Grid item>
              <Button
                onClick={logoutUser}
                variant="contained"
                color="secondary"
                disabled={isLoading}
                className={classes.logout__button}
              >
                {
                  isLoading
                    ? <CircularProgress size={16} className={classes.button__loading} />
                    : t('LOGIN.LOGOUT')
                }
              </Button>
            </Grid>
          </Grid>
        </Toolbar>
      </AppBar>
      <Menu open={openMenu} setOpen={setOpenMenu} />

      <main className={classes.content}>
        <Switch location={location}>
          <PrivateRoute path="/" exact>
            <Home />
          </PrivateRoute>
          <PrivateRoute path={['/movies/general', '/movies']} exact>
            <MoviesLoader Component={MoviesGeneral} />
          </PrivateRoute>
          <PrivateRoute path="/movies/people" exact>
            <MoviesLoader Component={MoviesPeople} />
          </PrivateRoute>
          <PrivateRoute path="/movies/graphs" exact>
            <MoviesLoader Component={MoviesGraphs} />
          </PrivateRoute>
          <PrivateRoute path="/movies/list" exact>
            <MoviesLoader Component={MoviesList} />
          </PrivateRoute>
          <PrivateRoute path="/jobs" exact>
            <Jobs />
          </PrivateRoute>
          <PrivateRoute path="/settings/general" exact>
            <GeneralSettings />
          </PrivateRoute>
          <PrivateRoute path="/settings/movie" exact>
            <MovieSettings />
          </PrivateRoute>
          <Route path="/login">
            <Login />
          </Route>
          <Route path="*">
            <NotFound />
          </Route>
        </Switch>
      </main>
    </div>
  );
};

export default withRouter(LoggedIn);
