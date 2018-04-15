import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { Actions, Effect } from '@ngrx/effects';
import { Store } from '@ngrx/store';
import { map, switchMap, catchError } from 'rxjs/operators';
import 'rxjs/add/observable/throw';

import { MovieService } from '../service/movie.service';
import { MovieActionTypes as MovieTypes, LoadMovieCollectionsAction, LoadGeneralStatsAction, LoadGeneralStatsSuccessAction, LoadMovieCollectionsSuccessAction } from './actions.movie';
import { SmallStat } from "../../shared/models/smallStat";
import { Collection } from "../../shared/models/collection";

import { EffectError } from '../../states/app.actions';

@Injectable()
export class MovieEffects {
  constructor(
    private actions$: Actions,
    private movieService: MovieService) {

    this.actions$.subscribe(data => {
      console.log(data.type);
    });
  }

  @Effect()
  getMovieGeneralStat$ = this.actions$
      .ofType(MovieTypes.LOAD_STATS_GENERAL)
    .pipe(
      map((data: LoadGeneralStatsAction) => data.payload),
      switchMap((list: string[]) => {
        return this.movieService.getGeneral(list);
      }),
      map((stats: SmallStat[]) => {
        return new LoadGeneralStatsSuccessAction(stats);
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
}
