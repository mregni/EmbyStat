import { Action } from '@ngrx/store';
import { Collection } from '../../shared/models/collection';
import { ShowStats } from '../models/showStats';
import { ShowGraphs } from '../models/showGraphs';
import { PersonStats } from '../../shared/models/personStats';
import { ShowCollectionRow } from '../models/showCollectionRow';

export enum ShowActionTypes {
  LOAD_COLLECTIONS = '[Shows] Load Show Collections',
  LOAD_COLLECTIONS_SUCCESS = '[Shows] Load Show Collections Success',
  LOAD_STATS_GENERAL = '[Shows] Load General Show Stats',
  LOAD_STATS_GENERAL_SUCCESS = '[Shows] Load General Show Stats Success',
  LOAD_STATS_PERSON = '[Shows] Load Person Stats',
  LOAD_STATS_PERSON_SUCCESS = '[Shows] Load Person Stats Success',
  LOAD_GRAPHS = '[Shows] Load Show Graphs',
  LOAD_GRAPHS_SUCCESS = '[Shows] Load Show Graphs Success',
  CLEAR_GRAPHS_SUCCESS = '[Shows] Clear Show Graphs Success',
  LOAD_COLLECTED_LIST = '[Shows] Load Collected List',
  LOAD_COLLECTED_LIST_SUCCESS = '[Shows] Load Collected List Success'
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

export class LoadPersonStatsAction implements Action {
  readonly type = ShowActionTypes.LOAD_STATS_PERSON;
  constructor(public payload: string[]) { }
}

export class LoadPersonStatsSuccessAction implements Action {
  readonly type = ShowActionTypes.LOAD_STATS_PERSON_SUCCESS;
  constructor(public payload: PersonStats) { }
}

export class LoadCollectedListAction implements Action {
  readonly type = ShowActionTypes.LOAD_COLLECTED_LIST;
  constructor(public payload: string[]) { }
}

export class LoadCollectedListSuccessAction implements Action {
  readonly type = ShowActionTypes.LOAD_COLLECTED_LIST_SUCCESS;
  constructor(public payload: ShowCollectionRow[]) { }
}

export type ShowActions = LoadShowCollectionsAction | LoadShowCollectionsSuccessAction
  | LoadGeneralStatsAction | LoadGeneralStatsSuccessAction
  | LoadGraphsAction | LoadGraphsSuccessAction | ClearGraphsSuccesAction
  | LoadPersonStatsAction | LoadPersonStatsSuccessAction
  | LoadCollectedListAction | LoadCollectedListSuccessAction;
