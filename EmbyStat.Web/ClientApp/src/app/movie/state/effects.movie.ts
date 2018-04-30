import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { Actions, Effect } from '@ngrx/effects';
import { map, switchMap, catchError } from 'rxjs/operators';
import 'rxjs/add/observable/throw';

import { MovieService } from '../service/movie.service';
import { MovieActionTypes as MovieTypes, LoadMovieCollectionsAction, LoadGeneralStatsAction, LoadDuplicateGraphSuccessAction, LoadGeneralStatsSuccessAction, LoadMovieCollectionsSuccessAction, LoadPersonStatsAction, LoadPersonStatsSuccessAction } from './actions.movie';
import { MovieStats } from "../models/movieStats";
import { MoviePersonStats } from "../models/moviePersonStats";
import { Collection } from "../../shared/models/collection";
import { Duplicate } from "../models/graphs/duplicate";

import { EffectError } from '../../states/app.actions';

@Injectable()
export class MovieEffects {
  constructor(
    private actions$: Actions,
    private movieService: MovieService) {
  }

  @Effect()
  getMovieGeneralStat$ = this.actions$
    .ofType(MovieTypes.LOAD_STATS_GENERAL)
    .pipe(
    map((data: LoadGeneralStatsAction) => data.payload),
    switchMap((list: string[]) => {
      return this.movieService.getGeneral(list);
    }),
    map((stats: MovieStats) => {
      return new LoadGeneralStatsSuccessAction(stats);
    }),
    catchError((err: any, caught: Observable<Object>) => Observable.throw(new EffectError(err)))
    );

  @Effect()
  getMoviePersonStat$ = this.actions$
    .ofType(MovieTypes.LOAD_STATS_PERSON)
    .pipe(
    map((data: LoadPersonStatsAction) => data.payload),
    switchMap((list: string[]) => {
      return this.movieService.getPerson(list);
    }),
    map((stats: MoviePersonStats) => {
      return new LoadPersonStatsSuccessAction(stats);
    }),
    catchError((err: any, caught: Observable<Object>) => Observable.throw(new EffectError(err)))
    );

  @Effect()
  getMovieCollections$ = this.actions$
    .ofType(MovieTypes.LOAD_MOVIE_COLLECTIONS)
    .pipe(
    map((data: LoadMovieCollectionsAction) => data.payload),
    switchMap(_ => {
      return this.movieService.getCollections();
    }),
    map((collections: Collection[]) => {
      return new LoadMovieCollectionsSuccessAction(collections);
    }),
    catchError((err: any, caught: Observable<Object>) => Observable.throw(new EffectError(err)))
    );

  @Effect()
  getDuplicateGraph$ = this.actions$
    .ofType(MovieTypes.LOAD_DUPLICATE_GRAPH)
    .pipe(
    map((data: LoadMovieCollectionsAction) => data.payload),
    switchMap((list: string[]) => {
      return this.movieService.getDuplicateGraph(list);
    }),
    map((list: Duplicate[]) => {
      return new LoadDuplicateGraphSuccessAction(list);
    }),
    catchError((err: any, caught: Observable<Object>) => Observable.throw(new EffectError(err)))
    );
}
