import { createSlice, PayloadAction } from '@reduxjs/toolkit';

import { RootState } from './RootReducer';
import { AppDispatch, AppThunk } from '.';
import { Wizard } from '../shared/models/wizard';
import { MediaServerUdpBroadcast, Library } from '../shared/models/mediaServer';

const initialState: Wizard = {
  serverAddress: '',
  serverName: '',
  serverPort: '',
  serverProtocol: 0,
  apiKey: '',
  serverType: 0,
  userId: '',
  serverBaseurl: '',
  serverBaseUrlNeeded: false,
  username: '',
  password: '',
  language: '',
  enableRollbarLogging: false,
  foundServers: [],
  searchedServers: false,
  allLibraries: [],
  movieLibraries: [],
  showLibraries: [],
  loadedMovieLibraryStep: false,
  loadedShowLibraryStep: false,
  serverId: '',
};

const wizardSlice = createSlice({
  name: 'wizard',
  initialState,
  reducers: {
    updateWizardState(state, action: PayloadAction<Wizard>) {
      return {
        ...action.payload,
      };
    },
    alreadyLoaded(state, action: PayloadAction) {
      return state;
    },
  },
});

export const setUser = (username: string, password: string): AppThunk => async (
  dispatch: AppDispatch,
  getState: () => RootState
) => {
  const wizard = { ...getState().wizard };
  wizard.username = username;
  wizard.password = password;
  dispatch(wizardSlice.actions.updateWizardState(wizard));
};

export const setServerConfiguration = (
  address: string,
  port: number,
  baseUrl: string,
  apiKey: string,
  type: number,
  protocol: number,
  baseUrlNeeded: boolean,
): AppThunk => async (dispatch: AppDispatch, getState: () => RootState) => {
  const wizard = { ...getState().wizard };
  wizard.serverAddress = address;
  wizard.serverPort = port;
  wizard.apiKey = apiKey;
  wizard.serverBaseurl = baseUrl;
  wizard.serverBaseUrlNeeded = baseUrlNeeded;
  wizard.serverProtocol = protocol;
  wizard.serverType = type;

  dispatch(wizardSlice.actions.updateWizardState(wizard));
};

export const setServerAddress = (
  address: string,
  port: number,
  protocol: number,
): AppThunk => async (dispatch: AppDispatch, getState: () => RootState) => {
  const wizard = { ...getState().wizard };
  wizard.serverAddress = address;
  wizard.serverPort = port;
  wizard.serverProtocol = protocol;

  dispatch(wizardSlice.actions.updateWizardState(wizard));
};

export const setlanguage = (language: string): AppThunk => async (
  dispatch: AppDispatch,
  getState: () => RootState
) => {
  const wizard = { ...getState().wizard };
  wizard.language = language;
  dispatch(wizardSlice.actions.updateWizardState(wizard));
};

export const setrollbar = (enabled: boolean): AppThunk => async (
  dispatch: AppDispatch,
  getState: () => RootState
) => {
  const wizard = { ...getState().wizard };
  wizard.enableRollbarLogging = enabled;
  dispatch(wizardSlice.actions.updateWizardState(wizard));
};

export const setAdminId = (adminId: string): AppThunk => async (
  dispatch: AppDispatch,
  getState: () => RootState
) => {
  const wizard = { ...getState().wizard };
  wizard.userId = adminId;
  dispatch(wizardSlice.actions.updateWizardState(wizard));
};

export const setAllLibraries = (libraries: Library[]): AppThunk => async (
  dispatch: AppDispatch,
  getState: () => RootState
) => {
  const wizard = { ...getState().wizard };
  wizard.allLibraries = libraries;
  dispatch(wizardSlice.actions.updateWizardState(wizard));
};

export const setFoundServers = (
  servers: MediaServerUdpBroadcast[],
  searched: boolean
): AppThunk => async (dispatch: AppDispatch, getState: () => RootState) => {
  const wizard = { ...getState().wizard };
  const newServers = [...servers];
  wizard.foundServers = newServers;
  wizard.searchedServers = searched;
  dispatch(wizardSlice.actions.updateWizardState(wizard));
};

export const updateSelectedLibraries = (
  libraries: string[],
  type: string
): AppThunk => async (dispatch: AppDispatch, getState: () => RootState) => {
  const wizard = { ...getState().wizard };
  if (type === 'movie') {
    wizard.movieLibraries = libraries;
  } else if (type === 'show') {
    wizard.showLibraries = libraries;
  }
  dispatch(wizardSlice.actions.updateWizardState(wizard));
};

export const setMovieLibraryStepLoaded = (loaded: boolean): AppThunk => async (
  dispatch: AppDispatch,
  getState: () => RootState
) => {
  const wizard = { ...getState().wizard };
  wizard.loadedMovieLibraryStep = loaded;
  dispatch(wizardSlice.actions.updateWizardState(wizard));
};

export const setShowLibraryStepLoaded = (loaded: boolean): AppThunk => async (
  dispatch: AppDispatch,
  getState: () => RootState
) => {
  const wizard = { ...getState().wizard };
  wizard.loadedShowLibraryStep = loaded;
  dispatch(wizardSlice.actions.updateWizardState(wizard));
};

export const setMediaServerId = (id: string): AppThunk => async (
  dispatch: AppDispatch,
  getState: () => RootState
) => {
  const wizard = { ...getState().wizard };
  wizard.serverId = id;
  dispatch(wizardSlice.actions.updateWizardState(wizard));
};

export default wizardSlice.reducer;
