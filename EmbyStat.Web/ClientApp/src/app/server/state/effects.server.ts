import { throwError, Observable, of } from 'rxjs';
import { Injectable } from '@angular/core';
import { Actions, Effect, ofType } from '@ngrx/effects';
import { Store } from '@ngrx/store';
import { map, switchMap, catchError, withLatestFrom } from 'rxjs/operators';


import { ServerInfo } from '../../shared/models//emby/server-info';
import { EmbyService } from '../../shared/services/emby.service';
import { ServerActionTypes, LoadServerInfoAction, LoadServerInfoSuccessAction, NoNeedServerInfoAction } from './actions.server';

import { ServerQuery } from './reducer.server';
import { EffectError } from '../../states/app.actions';
import { ApplicationState } from '../../states/app.state';

@Injectable()
export class ServerEffects {
  constructor(
    private actions$: Actions,
    private embyService: EmbyService,
    private store: Store<ApplicationState>) {
  }

  loaded$ = this.store.select(ServerQuery.getLoaded);

  @Effect()
  getServerInfo$ = this.actions$
    .pipe(
      ofType(ServerActionTypes.LOAD_SERVERINFO),
      map((data: LoadServerInfoAction) => data.payload),
      withLatestFrom(this.loaded$),
      switchMap(([_, loaded]) => {
        return loaded
          ? of(null)
          : this.embyService.getServerInfo();
      }),
      map((serverInfo: ServerInfo | null) => {
        return serverInfo
          ? new LoadServerInfoSuccessAction(serverInfo)
          : new NoNeedServerInfoAction();
      }),
      catchError((err: any, caught: Observable<Object>) => throwError(new EffectError(err)))
    );
}
