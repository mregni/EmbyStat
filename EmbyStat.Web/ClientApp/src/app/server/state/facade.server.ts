import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { Store } from '@ngrx/store';
import { Actions } from '@ngrx/effects';

import 'rxjs/add/observable/throw';

import { ServerInfo } from '../models/serverInfo';

import { ServerQuery } from './reducer.server';
import { LoadServerInfoAction } from './actions.server';

import { ApplicationState } from '../../states/app.state';

@Injectable()
export class ServerFacade {
  constructor(
    private actions$: Actions,
    private store: Store<ApplicationState>
  ) { }

  plugins$ = this.store.select(ServerQuery.getServerInfo);

  getServerInfo(): Observable<ServerInfo> {
    this.store.dispatch(new LoadServerInfoAction());
    return this.plugins$;
  }
}

