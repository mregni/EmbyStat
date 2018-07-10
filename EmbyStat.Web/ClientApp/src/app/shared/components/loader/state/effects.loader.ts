import { Injectable } from '@angular/core';
import { Actions, Effect } from '@ngrx/effects';
import { map } from 'rxjs/operators';
import 'rxjs/add/observable/throw';

import { ShowActionTypes } from '../../../../show/state/actions.show';
import { MovieActionTypes } from '../../../../movie/state/actions.movie';
import { HideLoaderShowGeneral, ShowLoaderShowGeneral,
  ShowLoaderShowCharts, HideLoaderShowCharts,
  ShowLoaderShowCollection, HideLoaderShowCollection,
  ShowLoaderMovieGeneral, HideLoaderMovieGeneral } from './actions.loader';


@Injectable()
export class LoaderEffects {
  constructor(
    private actions$: Actions) {
  }

  @Effect()
  hideShowGeneral = this.actions$
    .ofType(ShowActionTypes.LOAD_STATS_GENERAL_SUCCESS)
    .pipe(map(() => new HideLoaderShowGeneral()));

  @Effect()
  showShowGeneral = this.actions$
    .ofType(ShowActionTypes.LOAD_STATS_GENERAL)
    .pipe(map(() => new ShowLoaderShowGeneral()));

  @Effect()
  hideShowGraphs = this.actions$
    .ofType(ShowActionTypes.LOAD_GRAPHS_SUCCESS)
    .pipe(map(() => new HideLoaderShowCharts()));

  @Effect()
  showShowGraphs = this.actions$
    .ofType(ShowActionTypes.LOAD_STATS_GENERAL)
    .pipe(map(() => new ShowLoaderShowCharts()));

  @Effect()
  hideShowCollection = this.actions$
    .ofType(ShowActionTypes.LOAD_COLLECTIONS_SUCCESS)
    .pipe(map(() => new HideLoaderShowCollection()));

  @Effect()
  showShowCollection = this.actions$
    .ofType(ShowActionTypes.LOAD_COLLECTIONS)
    .pipe(map(() => new ShowLoaderShowCollection()));

  @Effect()
  hideMovieGeneral = this.actions$
    .ofType(MovieActionTypes.LOAD_STATS_GENERAL_SUCCESS)
    .pipe(map(() => new HideLoaderMovieGeneral()));

  @Effect()
  showMovieGeneral = this.actions$
    .ofType(MovieActionTypes.LOAD_STATS_GENERAL)
    .pipe(map(() => new ShowLoaderMovieGeneral()));
}
