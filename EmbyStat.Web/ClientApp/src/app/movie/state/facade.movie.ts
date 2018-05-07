import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { Store } from '@ngrx/store';
import 'rxjs/add/observable/throw';

import { MovieStats } from '../models/movieStats';
import { MoviePersonStats } from '../models/moviePersonStats';
import { Collection } from '../../shared/models/collection';
import { Graph } from '../../shared/models/graph';
import { Duplicate } from '../models/duplicate';

import { MovieQuery } from './reducer.movie';
import {
  LoadGeneralStatsAction, LoadMovieCollectionsAction,
  LoadPersonStatsAction, LoadDuplicateAction,
  LoadGraphsAction
} from './actions.movie';

import { ApplicationState } from '../../states/app.state';

@Injectable()
export class MovieFacade {
  constructor(
    private store: Store<ApplicationState>
  ) { }

  generalStats$ = this.store.select(MovieQuery.getGeneralStats);
  personStats$ = this.store.select(MovieQuery.getPersonStats);
  collections$ = this.store.select(MovieQuery.getMovieCollections);
  duplicates$ = this.store.select(MovieQuery.getDuplicates);
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

  getDuplicates(list: string[]): Observable<Duplicate[]> {
    this.store.dispatch(new LoadDuplicateAction(list));
    return this.duplicates$;
  }

  getGraphs(list: string[]): Observable<Graph[]> {
    this.store.dispatch(new LoadGraphsAction(list));
    return this.graphs$;
  }
}

