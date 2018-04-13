import { ApplicationState } from "../../states/app.state";

import { MovieStore } from '../models/movieStore';
import { GeneralStat } from '../models/generalStat';
import { MovieActions, MovieTypes } from './actions.movie';

const INITIAL_STATE_GENERAL_STAT: GeneralStat = {
  stats: []
}

const INITIAL_STATE: MovieStore = {
  generalStats: INITIAL_STATE_GENERAL_STAT
};

export function MovieReducer(state: MovieStore = INITIAL_STATE, action: MovieActions) {
  switch (action.type) {
    case MovieTypes.LOAD_STATS_GENERAL_SUCCESS:
    return {
      ...state,
      generalStats: { stats: action.payload } 
    };
  default:
    return state;
  }
}

export namespace MovieQuery {
  export const getGeneralStats = (state: ApplicationState) => state.movies.generalStats;
}
