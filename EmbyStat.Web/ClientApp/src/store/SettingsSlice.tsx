import { createSlice, PayloadAction } from '@reduxjs/toolkit';

import {
  MediaServerSettings,
  Settings,
  TvdbSettings,
} from '../shared/models/settings';
import {
  getSettings,
  updateSettings,
} from '../shared/services/SettingsService';

import { AppThunk } from '.';

const mediaServerSettings: MediaServerSettings = {
  authorizationScheme: 'mediaBrowser',
  serverAddress: '',
  serverName: '',
  serverPort: 0,
  serverProtocol: 0,
  apiKey: '',
  serverType: 0,
  userId: '',
  serverBaseurl: '',
  serverId: '',
};

const tvdbSettings: TvdbSettings = {
  apiKey: '',
  lastUpdate: null,
};

const initialState: Settings = {
  id: '',
  appName: 'EmbyStat',
  wizardFinished: true,
  language: 'en-US',
  toShortMovie: 10,
  keepLogsCount: 20,
  movieLibraries: [],
  showLibraries: [],
  autoUpdate: false,
  updateTrain: 2,
  updateInProgress: false,
  version: '0.0.0',
  mediaServer: mediaServerSettings,
  tvdb: tvdbSettings,
  enableRollbarLogging: false,
  isLoaded: false,
  toShortMovieEnabled: false,
  noUpdates: false,
  configDir: '',
  dataDir: '',
  logDir: '',
};

const settingsSlice = createSlice({
  name: 'settings',
  initialState,
  reducers: {
    receiveSettings(state, action: PayloadAction<Settings>) {
      return {
        ...action.payload,
        isLoaded: true,
      };
    },
    alreadyLoaded(state, action: PayloadAction) {
      return state;
    },
  },
});

export const loadSettings = (): AppThunk => async (
  dispatch,
  getState
) => {
  if (!getState().settings.isLoaded) {
    const settings = await getSettings();
    dispatch(settingsSlice.actions.receiveSettings(settings));
  } else {
    dispatch(settingsSlice.actions.alreadyLoaded());
  }
};

export const saveSettings = (settings: Settings): AppThunk => async (
  dispatch,
  getState
) => {
  dispatch(settingsSlice.actions.receiveSettings(settings));
  updateSettings(getState().settings);
};

export default settingsSlice.reducer;
