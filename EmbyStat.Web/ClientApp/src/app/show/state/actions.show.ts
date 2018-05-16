import { Action } from '@ngrx/store';
import { Collection } from '../../shared/models/collection';
import { ShowStats } from '../models/showStats';
import { ShowGraphs } from '../models/showGraphs';

export enum ShowActionTypes {
  LOAD_COLLECTIONS = '[Shows] Load Show Collections',
  LOAD_COLLECTIONS_SUCCESS = '[Shows] Load Show Collections Success',
  LOAD_STATS_GENERAL = '[Shows] Load General Show Stats',
  LOAD_STATS_GENERAL_SUCCESS = '[Shows] Load General Show Stats Success',
  LOAD_GRAPHS = '[Shows] Load Show Graphs',
  LOAD_GRAPHS_SUCCESS = '[Shows] Load Show Graphs Success',
  CLEAR_GRAPHS_SUCCESS = '[Shows] Clear Show Graphs Success'
}

export class LoadShowCollectionsAction implements Action {
  readonly type = ShowActionTypes.LOAD_COLLECTIONS;
  constructor(public payload = null) { }
}

export class LoadShowCollectionsSuccessAction implements Action {
  readonly type = ShowActionTypes.LOAD_COLLECTIONS_SUCCESS;
  constructor(public payload: Collection[]) { }
}

export class LoadGeneralStatsAction implements Action {
  readonly type = ShowActionTypes.LOAD_STATS_GENERAL;
  constructor(public payload: string[]) { }
}

export class LoadGeneralStatsSuccessAction implements Action {
  readonly type = ShowActionTypes.LOAD_STATS_GENERAL_SUCCESS;
  constructor(public payload: ShowStats) { }
}

export class LoadGraphsAction implements Action {
  readonly type = ShowActionTypes.LOAD_GRAPHS;
  constructor(public payload: string[]) { }
}

export class LoadGraphsSuccessAction implements Action {
  readonly type = ShowActionTypes.LOAD_GRAPHS_SUCCESS;
  constructor(public payload: ShowGraphs) { }
}

export class ClearGraphsSuccesAction implements Action {
  readonly type = ShowActionTypes.CLEAR_GRAPHS_SUCCESS;
  constructor(public payload = null) { }
}

export type ShowActions = LoadShowCollectionsAction | LoadShowCollectionsSuccessAction
  | LoadGeneralStatsAction | LoadGeneralStatsSuccessAction
  | LoadGraphsAction | LoadGraphsSuccessAction | ClearGraphsSuccesAction;
