import { ApplicationState } from '../../states/app.state';

import { ShowStore } from '../models/showStore';
import { ShowStats } from '../models/showStats';
import { ShowActions, ShowActionTypes } from './actions.show';

const INITIAL_STATE: ShowStore = {
  collections: [],
  showStats: new ShowStats()
};

export function ShowReducer(state: ShowStore = INITIAL_STATE, action: ShowActions) {
  switch (action.type) {
  case ShowActionTypes.LOAD_COLLECTIONS_SUCCESS:
    return {
      ...state,
      collections: action.payload
      };
    case ShowActionTypes.LOAD_STATS_GENERAL_SUCCESS:
      return {
        ...state,
        showStats: action.payload
      }
  default:
    return state;
  }
}

export namespace ShowQuery {
  export const getCollections = (state: ApplicationState) => state.shows.collections;
  export const getGeneralStats = (state: ApplicationState) => state.shows.showStats;
}
