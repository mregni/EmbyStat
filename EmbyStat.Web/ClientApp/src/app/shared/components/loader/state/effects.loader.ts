import { Injectable } from '@angular/core';
import { Actions, Effect } from '@ngrx/effects';
import { map } from 'rxjs/operators';
import 'rxjs/add/observable/throw';

import { ShowActionTypes } from '../../../../show/state/actions.show';
import { HideLoaderShowGeneral, ShowLoaderShowGeneral,
         ShowLoaderShowCharts, HideLoaderShowCharts } from './actions.loader';


@Injectable()
export class LoaderEffects {
  constructor(
    private actions$: Actions) {
  }

  @Effect()
  showShowGeneral = this.actions$
    .ofType(ShowActionTypes.LOAD_STATS_GENERAL_SUCCESS)
    .pipe(map(() => new HideLoaderShowGeneral()));

  @Effect()
  hideShowGeneral = this.actions$
    .ofType(ShowActionTypes.LOAD_STATS_GENERAL)
    .pipe(map(() => new ShowLoaderShowGeneral()));

  @Effect()
  showShowGraphs = this.actions$
    .ofType(ShowActionTypes.LOAD_GRAPHS_SUCCESS)
    .pipe(map(() => new HideLoaderShowCharts()));

  @Effect()
  hideShowGraphs = this.actions$
    .ofType(ShowActionTypes.LOAD_STATS_GENERAL)
    .pipe(map(() => new ShowLoaderShowCharts()));
}
