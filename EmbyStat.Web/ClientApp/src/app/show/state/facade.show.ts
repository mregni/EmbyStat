import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { Store } from '@ngrx/store';
import 'rxjs/add/observable/throw';

import { Collection } from '../../shared/models/collection';
import { ShowStats } from '../models/showStats';

import { ShowQuery } from './reducer.show';
import {
  LoadShowCollectionsAction, LoadGeneralStatsAction
  } from './actions.show';

import { ApplicationState } from '../../states/app.state';

@Injectable()
export class ShowFacade {
  constructor(
    private store: Store<ApplicationState>
  ) { }

  collections$ = this.store.select(ShowQuery.getCollections);
  generalStats$ = this.store.select(ShowQuery.getGeneralStats);

  getCollections(): Observable<Collection[]> {
    this.store.dispatch(new LoadShowCollectionsAction());
    return this.collections$;
  }

  getGeneralStats(list: string[]): Observable<ShowStats> {
    this.store.dispatch(new LoadGeneralStatsAction(list));
    return this.generalStats$;
  }
}
