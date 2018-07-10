import { Injectable } from '@angular/core';
import { Actions, Effect } from '@ngrx/effects';
import { map } from 'rxjs/operators';
import 'rxjs/add/observable/throw';
import { HideLoader, ShowLoader } from './actions.loader';
import {
  ShowActionTypes,
  LoadGeneralStatsAction,
  LoadGeneralStatsSuccessAction,
  } from '../../../../show/state/actions.show';


const showLoaderActions = [
  ShowActionTypes.LOAD_STATS_GENERAL
];

type ShowLoaderTypes = LoadGeneralStatsAction;

const hideLoaderActions = [
  ShowActionTypes.LOAD_STATS_GENERAL_SUCCESS
];

type HideLoaderTypes = LoadGeneralStatsSuccessAction;

@Injectable()
export class LoaderEffects {
  constructor(
    private actions$: Actions) {
  }

  @Effect()
  showSpinner = this.actions$
    .ofType<ShowLoaderTypes>(...showLoaderActions)
    .pipe(map(() => new ShowLoader()));

  @Effect()
  hideSpinner = this.actions$
    .ofType<HideLoaderTypes>(...hideLoaderActions)
    .pipe(map(() => new HideLoader()));
}
