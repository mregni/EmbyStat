import { ApplicationState } from '../../states/app.state';

import { About } from '../models/about';
import { AboutActions, AboutActionTypes } from './actions.about';

const INITIAL_STATE: About = {
  version: '',
  architecture: '',
  operatingSystem: '',
  operatingSystemVersion: '',
  isLoaded: false
};

export function AboutReducer(state: About = INITIAL_STATE, action: AboutActions) {
  switch (action.type) {
    case AboutActionTypes.LOAD_ABOUT_SUCCESS:
      return {
        ...state,
        version: action.payload.version,
        architecture: action.payload.architecture,
        operatingSystem: action.payload.operatingSystem,
        operatingSystemVersion: action.payload.operatingSystemVersion,
        isLoaded: true
      };
    default:
      return state;
  }
}

export namespace AboutQuery {
  export const getLoaded = (state: ApplicationState) => state.about.isLoaded;
  export const getAbout = (state: ApplicationState) => state.about;
}
