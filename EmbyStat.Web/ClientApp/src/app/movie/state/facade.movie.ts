import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { Store } from '@ngrx/store';
import { Actions } from '@ngrx/effects';

import 'rxjs/add/observable/throw';

import { GeneralStat } from '../models/generalStat';

import { MovieQuery } from './reducer.movie';
import { LoadGeneralStatsAction } from './actions.movie';

import { ApplicationState } from '../../states/app.state';

@Injectable()
export class MovieFacade {
  constructor(
    private actions$: Actions,
    private store: Store<ApplicationState>
  ) { }

  generalStats$ = this.store.select(MovieQuery.getGeneralStats);

  getServerInfo(): Observable<GeneralStat> {
    this.store.dispatch(new LoadGeneralStatsAction());
    return this.generalStats$;
  }
}

