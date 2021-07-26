import { createSlice, PayloadAction } from "@reduxjs/toolkit";

import { RootState } from "./RootReducer";
import { AppDispatch, AppThunk } from ".";
import { Wizard } from "../shared/models/wizard";
import { MediaServerUdpBroadcast, Library, MediaServerInfo } from "../shared/models/mediaServer";

const initialMediaServerInfoState: MediaServerInfo = {
  id: '',
  systemUpdateLevel: 0,
  operatingSystemDisplayName: '',
  hasPendingRestart: false,
  isShuttingDown: false,
  supportsLibraryMonitor: false,
  webSocketPortNumber: '',
  canSelfRestart: false,
  canSelfUpdate: false,
  canLaunchWebBrowser: false,
  programDataPath: '',
  itemsByNamePath: '',
  cachePath: '',
  logPath: '',
  internalMetadataPath: '',
  transcodingTempPath: '',
  httpServerPortNumber: 0,
  supportsHttps: false,
  httpsPortNumber: '',
  hasUpdateAvailable: false,
  supportsAutoRunAtStartup: false,
  hardwareAccelerationRequiresPremiere: false,
  localAddress: '',
  wanAddress: '',
  serverName: '',
  version: '',
  operatingSystem: '',
  isLoaded: false
}

const initialState: Wizard = {
  serverAddress: "",
  serverName: "",
  serverPort: null,
  serverProtocol: 0,
  apiKey: "",
  serverType: 0,
  userId: "",
  serverBaseurl: "",
  serverBaseUrlNeeded: false,
  username: "",
  password: "",
  language: "",
  languages: [],
  enableRollbarLogging: false,
  foundServers: [],
  searchedServers: false,
  allLibraries: [],
  movieLibraries: [],
  showLibraries: [],
  serverId: "",
  administrators: [],
  mediaServerInfo: initialMediaServerInfoState,
  fireSync: false,
};

const wizardSlice = createSlice({
  name: "wizard",
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
  port: number | null,
  baseUrl: string,
  apiKey: string,
  type: number,
  protocol: number,
  baseUrlNeeded: boolean
): AppThunk => async (dispatch: AppDispatch, getState: () => RootState) => {
  const wizard = { ...getState().wizard };
  wizard.serverAddress = address;
  wizard.serverPort = port;
  wizard.apiKey = apiKey;
  wizard.serverBaseurl = baseUrl;
  wizard.serverBaseUrlNeeded = baseUrlNeeded;
  // wizard.serverProtocol = protocol;
  // wizard.serverType = type;

  dispatch(wizardSlice.actions.updateWizardState(wizard));
};

export const setServerAddress = (
  address: string,
  port: number,
  protocol: number
): AppThunk => async (dispatch: AppDispatch, getState: () => RootState) => {
  const wizard = { ...getState().wizard };
  wizard.serverAddress = address;
  wizard.serverPort = port;
  // wizard.serverProtocol = protocol;

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
  if (type === "movie") {
    wizard.movieLibraries = libraries;
  } else if (type === "show") {
    wizard.showLibraries = libraries;
  }
  dispatch(wizardSlice.actions.updateWizardState(wizard));
};

export const setMovieLibraryStepLoaded = (loaded: boolean): AppThunk => async (
  dispatch: AppDispatch,
  getState: () => RootState
) => {
  const wizard = { ...getState().wizard };
  dispatch(wizardSlice.actions.updateWizardState(wizard));
};

export const setShowLibraryStepLoaded = (loaded: boolean): AppThunk => async (
  dispatch: AppDispatch,
  getState: () => RootState
) => {
  const wizard = { ...getState().wizard };
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
