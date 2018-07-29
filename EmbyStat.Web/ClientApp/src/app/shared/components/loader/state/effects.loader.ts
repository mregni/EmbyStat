import { Injectable } from '@angular/core';
import { Actions, Effect } from '@ngrx/effects';
import { map } from 'rxjs/operators';
import 'rxjs/add/observable/throw';

import { MovieActionTypes } from '../../../../movie/state/actions.movie';
import { HideLoaderShowGeneral, ShowLoaderShowGeneral,
  ShowLoaderShowCharts, HideLoaderShowCharts,
  ShowLoaderShowCollection, HideLoaderShowCollection,
  ShowLoaderMovieGeneral, HideLoaderMovieGeneral,
  ShowLoaderMovieGraphs, HideLoaderMovieGraphs,
  ShowLoaderMoviePeople, HideLoaderMoviePeople,
  ShowLoaderMovieSuspicious, HideLoaderMovieSuspicious } from './actions.loader';


@Injectable()
export class LoaderEffects {
  constructor(
    private actions$: Actions) {
  }

  @Effect()
  hideMovieGeneral = this.actions$
    .ofType(MovieActionTypes.LOAD_STATS_GENERAL_SUCCESS)
    .pipe(map(() => new HideLoaderMovieGeneral()));

  @Effect()
  showMovieGeneral = this.actions$
    .ofType(MovieActionTypes.LOAD_STATS_GENERAL)
    .pipe(map(() => new ShowLoaderMovieGeneral()));

  @Effect()
  hideMovieGraphs = this.actions$
    .ofType(MovieActionTypes.LOAD_GRAPHS_SUCCESS)
    .pipe(map(() => new HideLoaderMovieGraphs()));

  @Effect()
  showMovieGraphs = this.actions$
    .ofType(MovieActionTypes.LOAD_GRAPHS)
  .pipe(map(() => new ShowLoaderMovieGraphs()));

  @Effect()
  hideMoviePeople = this.actions$
    .ofType(MovieActionTypes.LOAD_STATS_PERSON_SUCCESS)
    .pipe(map(() => new HideLoaderMoviePeople()));

  @Effect()
  showMoviePeople = this.actions$
    .ofType(MovieActionTypes.LOAD_STATS_PERSON)
  .pipe(map(() => new ShowLoaderMoviePeople()));

  @Effect()
  hideMovieSuspicious = this.actions$
    .ofType(MovieActionTypes.LOAD_SUSPICIOUS_SUCCESS)
    .pipe(map(() => new HideLoaderMovieSuspicious()));

  @Effect()
  showMovieSuspicious = this.actions$
    .ofType(MovieActionTypes.LOAD_SUSPICIOUS)
    .pipe(map(() => new ShowLoaderMovieSuspicious()));
}
