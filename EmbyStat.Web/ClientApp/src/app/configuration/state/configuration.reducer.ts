import { createSelector } from '@ngrx/store';
import { ApplicationState } from "../../states/app.state";

import { Configuration } from '../models/configuration';
import {
  ConfigurationActionTypes,
  ConfigurationActions
} from './configuration.actions';


const INITIAL_STATE: Configuration = {
  wizardFinished: true,
  accessToken: "",
  embyUserName: "",
  embyServerAddress: "",
  username: "",
  userId: "",
  language: "en"
}

export function configurationReducer(state: Configuration = INITIAL_STATE, action: ConfigurationActions) {
  switch (action.type) {
    case ConfigurationActionTypes.LOAD_CONFIGURATION_SUCCESS:
      return {
        ...state,
        language: action.payload.language,
        wizardFinished: action.payload.wizardFinished
      };
    default:
      return state;
  }
}

export namespace ConfigurationQuery {
  export const getConfiguration = (state: ApplicationState) => state.configuration;
}
