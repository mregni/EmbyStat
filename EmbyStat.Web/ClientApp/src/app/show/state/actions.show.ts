import { Action } from '@ngrx/store';
import { Collection } from '../../shared/models/collection';
import { ShowStats } from '../models/showStats';

export enum ShowActionTypes {
  LOAD_COLLECTIONS = '[ShowCollections] Load Show Collections',
  LOAD_COLLECTIONS_SUCCESS = '[ShowCollections] Load Show Collections Success',
  LOAD_STATS_GENERAL = '[ShowGeneralStats] Load General Show Stats',
  LOAD_STATS_GENERAL_SUCCESS = '[ShowGeneralStats] Load General Show Stats Success'
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

export type ShowActions = LoadShowCollectionsAction | LoadShowCollectionsSuccessAction
  | LoadGeneralStatsAction | LoadGeneralStatsSuccessAction;
