import React, {useContext, useEffect} from 'react';
import {Navigate, Route, Routes, useNavigate} from 'react-router';

import {RequireAuth} from './authentication';
import i18n from './i18n';
import About from './pages/about';
import {Home} from './pages/home';
import Jobs from './pages/jobs';
import Login from './pages/login';
import Logs from './pages/logs';
import {General as MovieGeneral, List as MovieList, Movies} from './pages/movies';
import {Server} from './pages/server';
import {
  Movies as MovieSettings, Server as ServerSettings, Settings, Shows as ShowSettings,
} from './pages/settings';
import {General as ShowGeneral, List as ShowList, Shows} from './pages/shows';
import {Users} from './pages/users';
import {UserDetails} from './pages/users/subpages';
import Wizard from './pages/wizard';
import {SettingsContext} from './shared/context/settings';

function RoutesContainer() {
  const {userConfig, load} = useContext(SettingsContext);
  const navigate = useNavigate();

  useEffect(() => {
    if (userConfig !== null) {
      i18n.changeLanguage(userConfig.language);
      if (!userConfig.wizardFinished) {
        navigate('/wizard');
      }
    }
  }, [load, userConfig, navigate]);

  return (
    <Routes>
      <Route path="/login" element={<Login />} />
      <Route path="/movies" element={<RequireAuth><Movies /></RequireAuth>}>
        <Route path="" element={<MovieGeneral />} />
        <Route path="list" element={<MovieList />} />
      </Route>
      <Route path="/shows" element={<RequireAuth><Shows /></RequireAuth>}>
        <Route path="" element={<ShowGeneral />} />
        <Route path="list" element={<ShowList />} />
      </Route>
      <Route path="/jobs" element={<RequireAuth><Jobs /></RequireAuth>} />
      <Route path="/server" element={<RequireAuth><Server /></RequireAuth>} />
      <Route path="/home" element={<RequireAuth><Home /></RequireAuth>} />
      <Route path="/logs" element={<RequireAuth><Logs /></RequireAuth>} />
      <Route path="/settings" element={<RequireAuth><Settings /></RequireAuth>}>
        <Route path="" element={<ServerSettings />} />
        <Route path="show" element={<ShowSettings />} />
        <Route path="movie" element={<MovieSettings />} />
      </Route>
      <Route path="/users" element={<RequireAuth><Users /></RequireAuth>} />
      <Route path="/users/:id" element={<RequireAuth><UserDetails /></RequireAuth>} />
      <Route path="/about" element={<RequireAuth><About /></RequireAuth>} />
      <Route path="/wizard" element={<Wizard />} />
      <Route path="/" element={<Navigate to="/home" replace={true} />} />
    </Routes>
  );
}

export default RoutesContainer;
