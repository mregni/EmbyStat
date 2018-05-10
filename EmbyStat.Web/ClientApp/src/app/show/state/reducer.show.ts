import { ApplicationState } from '../../states/app.state';

import { ShowStore } from '../models/showStore';
import { ShowActions, ShowActionTypes } from './actions.show';

const INITIAL_STATE: ShowStore = {
  collections: []
};

export function ShowReducer(state: ShowStore = INITIAL_STATE, action: ShowActions) {
  switch (action.type) {
  case ShowActionTypes.LOAD_COLLECTIONS_SUCCESS:
    return {
      ...state,
      collections: action.payload
    };
  default:
    return state;
  }
}

export namespace ShowQuery {
  export const getCollections = (state: ApplicationState) => state.shows.collections;
}
