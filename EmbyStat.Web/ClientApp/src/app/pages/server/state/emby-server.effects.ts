
import { Observable, of, throwError } from 'rxjs';
import { catchError, map, switchMap, withLatestFrom } from 'rxjs/operators';

import { Injectable } from '@angular/core';
import { Actions, Effect, ofType } from '@ngrx/effects';
import { Store } from '@ngrx/store';

import { ServerInfo } from '../../../shared/models/media-server/server-info';
import { MediaServerService } from '../../../shared/services/media-server.service';
import { EffectError } from '../../../states/app.actions';
import { ApplicationState } from '../../../states/app.state';
import {
    EmbyServerActionTypes, LoadEmbyServerInfo, LoadEmbyServerInfoSuccess, NoNeedEmbyServerInfo
} from './emby-server.actions';
import { EmbyServerInfoQuery } from './emby-server.reducer';

@Injectable()
export class EmbyServerInfoEffects {
  constructor(
    private actions$: Actions,
    private mediaServerService: MediaServerService,
    private store: Store<ApplicationState>) {
  }

  loaded$ = this.store.select(EmbyServerInfoQuery.getLoaded);

  @Effect()
  getEmbyServerInfo$ = this.actions$
    .pipe(
      ofType(EmbyServerActionTypes.LOAD_EMBY_SERVER_INFO),
      map((data: LoadEmbyServerInfo) => data.payload),
      withLatestFrom(this.loaded$),
      switchMap(([_, loaded]) => {
        return loaded
          ? of(null)
          : this.mediaServerService.getEmbyServerInfo();
      }),
      map((serverInfo: ServerInfo | null) => {
        return serverInfo
          ? new LoadEmbyServerInfoSuccess(serverInfo)
          : new NoNeedEmbyServerInfo();
      }),
      catchError((err: any, caught: Observable<Object>) => throwError(new EffectError(err)))
    );
}
