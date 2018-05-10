import { Action } from '@ngrx/store';
import { Collection } from '../../shared/models/collection';

export enum ShowActionTypes {
  LOAD_COLLECTIONS = '[ShowCollections] Load Show Collections',
  LOAD_COLLECTIONS_SUCCESS = '[ShowCollections] Load Show Collections Success'
}

export class LoadShowCollectionsAction implements Action {
  readonly type = ShowActionTypes.LOAD_COLLECTIONS;
  constructor(public payload = null) { }
}

export class LoadShowCollectionsSuccessAction implements Action {
  readonly type = ShowActionTypes.LOAD_COLLECTIONS_SUCCESS;
  constructor(public payload: Collection[]) { }
}

export type ShowActions = LoadShowCollectionsAction | LoadShowCollectionsSuccessAction;
