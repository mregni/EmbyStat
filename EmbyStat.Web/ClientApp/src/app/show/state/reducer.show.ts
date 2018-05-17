import { ApplicationState } from '../../states/app.state';

import { ShowStore } from '../models/showStore';
import { ShowStats } from '../models/showStats';
import { ShowGraphs } from '../models/showGraphs';
import { PersonStats } from '../../shared/models/personStats';
import { ShowActions, ShowActionTypes } from './actions.show';

const INITIAL_STATE: ShowStore = {
  collections: [],
  showStats: new ShowStats(),
  graphs: new ShowGraphs(),
  personStats: new PersonStats()
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
      };
    case ShowActionTypes.LOAD_GRAPHS_SUCCESS:
      console.log(action.payload);
      return {
        ...state,
        graphs: action.payload
      };
    case ShowActionTypes.CLEAR_GRAPHS_SUCCESS:
      return {
        ...state,
        graphs: new ShowGraphs()
      };
    case ShowActionTypes.LOAD_STATS_PERSON_SUCCESS:
      return {
        ...state,
        personStats: action.payload
      }
  default:
    return state;
  }
}

export namespace ShowQuery {
  export const getCollections = (state: ApplicationState) => state.shows.collections;
  export const getGeneralStats = (state: ApplicationState) => state.shows.showStats;
  export const getGraphs = (state: ApplicationState) => state.shows.graphs;
  export const getPersonStats = (state: ApplicationState) => state.shows.personStats;
}
