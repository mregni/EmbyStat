import { Action } from '@ngrx/store';
import { MovieStats } from "../models/movieStats";
import { Collection } from "../../shared/models/collection";

export enum MovieActionTypes {
  LOAD_STATS_GENERAL = '[MovieGeneralStat] Load General Movie Stats',
  LOAD_STATS_GENERAL_SUCCESS = '[MovieGeneralStat] Load General Movie Stats Success',
  LOAD_MOVIE_COLLECTIONS = '[MovieCollections] Load Movie Collections',
  LOAD_MOVIE_COLLECTIONS_SUCCESS = '[MovieCollections] Load Movie Collections Success'
}

export class LoadGeneralStatsAction implements Action {
  readonly type = MovieActionTypes.LOAD_STATS_GENERAL;
  constructor(public payload: string[]) { }
}

export class LoadGeneralStatsSuccessAction implements Action {
  readonly type = MovieActionTypes.LOAD_STATS_GENERAL_SUCCESS;
  constructor(public payload: MovieStats) { }
}

export class LoadMovieCollectionsAction implements Action {
  readonly type = MovieActionTypes.LOAD_MOVIE_COLLECTIONS;
  constructor(public payload = null) { }
}

export class LoadMovieCollectionsSuccessAction implements Action {
  readonly type = MovieActionTypes.LOAD_MOVIE_COLLECTIONS_SUCCESS;
  constructor(public payload: Collection[]) { }
}

export type MovieActions = LoadGeneralStatsAction | LoadGeneralStatsSuccessAction |
                           LoadMovieCollectionsAction | LoadMovieCollectionsSuccessAction;
