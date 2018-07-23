import { ApplicationState } from '../../states/app.state';

import { Configuration } from '../models/configuration';
import {
  ConfigurationActionTypes,
  ConfigurationActions
} from './actions.configuration';

const INITIAL_STATE: Configuration = {
  wizardFinished: true,
  accessToken: '',
  embyUserName: '',
  embyServerAddress: '',
  username: '',
  language: 'en-US',
  serverName: '',
  isLoaded: false,
  toShortMovie: 10,
  embyUserId: '',
  id: '',
  movieCollectionTypes: [],
  showCollectionTypes: []
};

export function configurationReducer(state: Configuration = INITIAL_STATE, action: ConfigurationActions) {
  switch (action.type) {
    case ConfigurationActionTypes.LOAD_CONFIGURATION_SUCCESS:
      return {
        ...state,
        language: action.payload.language,
        wizardFinished: action.payload.wizardFinished,
        username: action.payload.username,
        accessToken: action.payload.accessToken,
        embyServerAddress: action.payload.embyServerAddress,
        embyUserName: action.payload.embyUserName,
        serverName: action.payload.serverName,
        embyUserId: action.payload.embyUserId,
        toShortMovie: action.payload.toShortMovie,
        id: action.payload.id,
        movieCollectionTypes: action.payload.movieCollectionTypes,
        showCollectionTypes: action.payload.showCollectionTypes,
        isLoaded: true
      };
    case ConfigurationActionTypes.UPDATE_CONFIGURATION_SUCCESS:
      return {
        ...state,
        language: action.payload.language,
        wizardFinished: action.payload.wizardFinished,
        username: action.payload.username,
        accessToken: action.payload.accessToken,
        embyServerAddress: action.payload.embyServerAddress,
        embyUserName: action.payload.embyUserName,
        serverName: action.payload.serverName,
        embyUserId: action.payload.embyUserId,
        toShortMovie: action.payload.toShortMovie,
        id: action.payload.id,
        movieCollectionTypes: action.payload.movieCollectionTypes,
        showCollectionTypes: action.payload.showCollectionTypes,
        isLoaded: true
      };
    default:
      return state;
  }
}

export namespace ConfigurationQuery {
  export const getConfiguration = (state: ApplicationState) => state.configuration;
  export const getLoaded = (state: ApplicationState) => state.configuration.isLoaded;
}
