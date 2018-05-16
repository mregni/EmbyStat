import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { Store } from '@ngrx/store';
import 'rxjs/add/observable/throw';

import { MovieStats } from '../models/movieStats';
import { MoviePersonStats } from '../models/moviePersonStats';
import { Collection } from '../../shared/models/collection';
import { MovieGraphs } from '../models/movieGraphs';
import { SuspiciousMovies } from '../models/suspiciousMovies';

import { MovieQuery } from './reducer.movie';
import {
  LoadGeneralStatsAction, LoadMovieCollectionsAction,
  LoadPersonStatsAction, LoadSuspiciousAction,
  LoadGraphsAction, ClearGraphsSuccesAction
} from './actions.movie';

import { ApplicationState } from '../../states/app.state';

@Injectable()
export class MovieFacade {
  constructor(
    private store: Store<ApplicationState>
  ) { }

  generalStats$ = this.store.select(MovieQuery.getGeneralStats);
  personStats$ = this.store.select(MovieQuery.getPersonStats);
  collections$ = this.store.select(MovieQuery.getCollections);
  suspicious$ = this.store.select(MovieQuery.getSuspicious);
  graphs$ = this.store.select(MovieQuery.getGraphs);

  getGeneralStats(list: string[]): Observable<MovieStats> {
    this.store.dispatch(new LoadGeneralStatsAction(list));
    return this.generalStats$;
  }

  getPeopleStats(list: string[]): Observable<MoviePersonStats> {
    this.store.dispatch(new LoadPersonStatsAction(list));
    return this.personStats$;
  }

  getCollections(): Observable<Collection[]> {
    this.store.dispatch(new LoadMovieCollectionsAction());
    return this.collections$;
  }

  getDuplicates(list: string[]): Observable<SuspiciousMovies> {
    this.store.dispatch(new LoadSuspiciousAction(list));
    return this.suspicious$;
  }

  getGraphs(list: string[]): Observable<MovieGraphs> {
    this.store.dispatch(new LoadGraphsAction(list));
    return this.graphs$;
  }

  clearGraphs(): void {
    this.store.dispatch(new ClearGraphsSuccesAction);
  }
}

