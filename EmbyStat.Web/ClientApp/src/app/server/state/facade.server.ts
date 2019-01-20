import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { Store } from '@ngrx/store';

import 'rxjs/add/observable/throw';

import { ServerInfo } from '../../shared/models/emby/server-info';

import { ServerQuery } from './reducer.server';
import { LoadServerInfoAction } from './actions.server';

import { ApplicationState } from '../../states/app.state';

@Injectable()
export class ServerFacade {
  constructor(private store: Store<ApplicationState>) { }

  serverInfo$ = this.store.select(ServerQuery.getServerInfo);

  getServerInfo(): Observable<ServerInfo> {
    this.store.dispatch(new LoadServerInfoAction());
    return this.serverInfo$;
  }
}

