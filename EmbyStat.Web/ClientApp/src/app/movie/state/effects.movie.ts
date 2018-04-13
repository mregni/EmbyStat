import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { Actions, Effect } from '@ngrx/effects';
import { Store } from '@ngrx/store';
import { map, switchMap, catchError } from 'rxjs/operators';
import { of } from 'rxjs/observable/of';
import 'rxjs/add/observable/throw';

import { MovieService } from '../service/movie.service';
import { MovieTypes, LoadGeneralStatsSuccessAction } from './actions.movie';
import { MovieQuery } from './reducer.movie';
import { SmallStat } from "../../shared/models/smallStat";

import { EffectError } from '../../states/app.actions';
import { ApplicationState } from '../../states/app.state';

@Injectable()
export class MovieEffects {
  constructor(
    private actions$: Actions,
    private movieService: MovieService,
    private store: Store<ApplicationState>) {
  }

  @Effect()
  getPlugins$ = this.actions$
      .ofType(MovieTypes.LOAD_STATS_GENERAL)
    .pipe(
      map((data: any) => data.payload),
      switchMap(_ => {
        return this.movieService.getGeneral();
      }),
      map((stats: SmallStat[] | null) => {
        return new LoadGeneralStatsSuccessAction(stats);
      }),
      catchError((err: any, caught: Observable<Object>) => Observable.throw(new EffectError(err)))
    );
}
