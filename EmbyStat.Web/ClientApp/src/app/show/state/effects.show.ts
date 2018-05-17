import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { Actions, Effect } from '@ngrx/effects';
import { map, switchMap, catchError } from 'rxjs/operators';
import 'rxjs/add/observable/throw';

import { ShowService } from '../service/show.service';
import {
  ShowActionTypes,
  LoadShowCollectionsAction,
  LoadShowCollectionsSuccessAction,
  LoadGeneralStatsAction,
  LoadGeneralStatsSuccessAction,
  LoadGraphsAction,
  LoadGraphsSuccessAction,
  LoadPersonStatsAction,
  LoadPersonStatsSuccessAction
} from './actions.show';
import { Collection } from '../../shared/models/collection';
import { ShowStats } from '../models/showStats';
import { ShowGraphs } from '../models/showGraphs';
import { PersonStats } from '../../shared/models/personStats';

import { EffectError } from '../../states/app.actions';

@Injectable()
export class ShowEffects {
  constructor(
    private actions$: Actions,
    private showService: ShowService) {
  }

  @Effect()
  getShowCollections$ = this.actions$
    .ofType(ShowActionTypes.LOAD_COLLECTIONS)
    .pipe(
    map((data: LoadShowCollectionsAction) => data.payload),
    switchMap(_ => {
      return this.showService.getCollections();
    }),
    map((collections: Collection[]) => {
      return new LoadShowCollectionsSuccessAction(collections);
    }),
    catchError((err: any, caught: Observable<Object>) => Observable.throw(new EffectError(err)))
    );

  @Effect()
  getGeneralStats$ = this.actions$
    .ofType(ShowActionTypes.LOAD_STATS_GENERAL)
    .pipe(
    map((data: LoadGeneralStatsAction) => data.payload),
    switchMap((list: string[]) => {
      return this.showService.getGeneralStats(list);
    }),
    map((stats: ShowStats) => {
      return new LoadGeneralStatsSuccessAction(stats);
    }),
    catchError((err: any, caught: Observable<Object>) => Observable.throw(new EffectError(err)))
  );

  @Effect()
  getGraphs$ = this.actions$
    .ofType(ShowActionTypes.LOAD_GRAPHS)
    .pipe(
      map((data: LoadGraphsAction) => data.payload),
      switchMap((list: string[]) => {
        return this.showService.getGraphs(list);
      }),
      map((graphs: ShowGraphs) => {
        return new LoadGraphsSuccessAction(graphs);
      }),
      catchError((err: any, caught: Observable<Object>) => Observable.throw(new EffectError(err)))
  );

  @Effect()
  getPersonStats$ = this.actions$
    .ofType(ShowActionTypes.LOAD_STATS_PERSON)
    .pipe(
      map((data: LoadPersonStatsAction) => data.payload),
      switchMap((list: string[]) => {
        return this.showService.getPersonStats(list);
      }),
      map((stats: PersonStats) => {
        return new LoadPersonStatsSuccessAction(stats);
      }),
      catchError((err: any, caught: Observable<Object>) => Observable.throw(new EffectError(err)))
    );
}
