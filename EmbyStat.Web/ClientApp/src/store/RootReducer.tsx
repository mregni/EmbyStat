import { combineReducers } from '@reduxjs/toolkit';
import settingsSlice from './SettingsSlice';
import languageSlice from './LanguageSlice';
import jobSlice from './JobSlice';
import jobLogsSlice from './JobLogsSlice';
import movieSlice from './MovieSlice';
import showSlice from './ShowSlice';
import serverStatusSlice from './ServerStatusSlice';

const rootReducer = combineReducers({
  settings: settingsSlice,
  languages: languageSlice,
  jobs: jobSlice.reducer,
  jobLogs: jobLogsSlice.reducer,
  movies: movieSlice,
  shows: showSlice,
  serverStatus: serverStatusSlice.reducer
});

export type RootState = ReturnType<typeof rootReducer>;

export default rootReducer;
