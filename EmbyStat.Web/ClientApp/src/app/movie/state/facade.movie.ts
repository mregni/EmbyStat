import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { Store } from '@ngrx/store';
import { Actions } from '@ngrx/effects';

import 'rxjs/add/observable/throw';

import { SmallStat } from '../../shared/models/smallStat';
import { Collection } from '../../shared/models/collection';

import { MovieQuery } from './reducer.movie';
import { LoadGeneralStatsAction, LoadMovieCollectionsAction } from './actions.movie';

import { ApplicationState } from '../../states/app.state';

@Injectable()
export class MovieFacade {
  constructor(
    private store: Store<ApplicationState>
  ) { }

  generalStats$ = this.store.select(MovieQuery.getGeneralStats);
  collections$ = this.store.select(MovieQuery.getMovieCollections);

  getGeneralStats(list: string[]): Observable<SmallStat[]> {
    this.store.dispatch(new LoadGeneralStatsAction(list));
    return this.generalStats$;
  }

  getCollections(): Observable<Collection[]> {
    this.store.dispatch(new LoadMovieCollectionsAction());
    return this.collections$;
  }
}

