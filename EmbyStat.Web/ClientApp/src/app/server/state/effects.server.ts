import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { Actions, Effect } from '@ngrx/effects';
import { Store } from '@ngrx/store';
import { map, switchMap, catchError, withLatestFrom } from 'rxjs/operators';
import { of } from 'rxjs/observable/of';

import 'rxjs/add/observable/throw';

import { ServerInfo } from '../models/serverInfo';
import { ServerService } from '../service/server.service';
import { ServerActionTypes, LoadServerInfoAction, LoadServerInfoSuccessAction, NoNeedServerInfoAction } from './actions.server';

import { ServerQuery } from './reducer.server';
import { EffectError } from '../../states/app.actions';
import { ApplicationState } from '../../states/app.state';
import { NoopAction } from '../../states/app.actions';

@Injectable()
export class ServerEffects {
  constructor(  
    private actions$: Actions,
    private serverService: ServerService,
    private store: Store<ApplicationState>) {
  }

  public loaded$ = this.store.select(ServerQuery.getLoaded);

  @Effect()
  getPlugins$ = this.actions$
    .ofType(ServerActionTypes.LOAD_SERVERINFO)
    .pipe(
      map((data: LoadServerInfoAction) => data.payload),
      withLatestFrom(this.loaded$),
      switchMap(([_, loaded]) => {
        return loaded
          ? of(null)
          : this.serverService.getServerInfo();
      }),
      map((serverInfo: ServerInfo | null) => {
        return serverInfo
          ? new LoadServerInfoSuccessAction(serverInfo)
          : new NoNeedServerInfoAction();
      }),
      catchError((err: any, caught: Observable<Object>) => Observable.throw(new EffectError(err)))
    );
}
