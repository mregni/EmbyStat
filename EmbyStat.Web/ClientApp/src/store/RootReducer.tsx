import { combineReducers } from '@reduxjs/toolkit';
import settingsSlice from './SettingsSlice';
import languageSlice from './LanguageSlice';
import wizardSlice from './WizardSlice';
import jobSlice from './JobSlice';
import jobLogsSlice from './JobLogsSlice';
import movieSlice from './MovieSlice';
import serverStatusSlice from './ServerStatusSlice';

const rootReducer = combineReducers({
  settings: settingsSlice,
  languages: languageSlice,
  wizard: wizardSlice,
  jobs: jobSlice.reducer,
  jobLogs: jobLogsSlice.reducer,
  movies: movieSlice,
  serverStatus: serverStatusSlice.reducer
});

export type RootState = ReturnType<typeof rootReducer>;

export default rootReducer;
