import { Observable } from 'rxjs';
import { EmbyServerInfoQuery } from 'src/app/pages/server/state/emby-server.reducer';

import { Injectable } from '@angular/core';
import { Store } from '@ngrx/store';

import { LoadEmbyServerInfo } from '../../pages/server/state/emby-server.actions';
import { ApplicationState } from '../../states/app.state';
import { Language } from '../models/language';
import { ServerInfo } from '../models/media-server/server-info';

@Injectable()
export class EmbyServerInfoFacade {
  constructor(private store: Store<ApplicationState>) { }

  embyServerInfo$ = this.store.select(EmbyServerInfoQuery.getInfo);

  getEmbyServerInfo(): Observable<ServerInfo> {
    this.store.dispatch(new LoadEmbyServerInfo());
    return this.embyServerInfo$;
  }
}

