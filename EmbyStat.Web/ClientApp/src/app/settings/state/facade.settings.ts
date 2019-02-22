import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Store } from '@ngrx/store';



import { Settings } from '../models/settings';
import { EmbyUdpBroadcast } from '../../shared/models/emby/emby-udp-broadcast';
import { EmbyToken } from '../../shared/models/emby/emby-token';
import { EmbyLogin } from '../../shared/models/emby/emby-login';
import { EmbyService } from '../../shared/services/emby.service';

import { SettingsQuery } from './reducer.settings';
import { LoadSettingsAction, UpdateSettingsAction } from './actions.settings';

import { ApplicationState } from '../../states/app.state';

@Injectable()
export class SettingsFacade {
  constructor(
    private store: Store<ApplicationState>,
    private embyService: EmbyService
  ) { }

  settings$ = this.store.select(SettingsQuery.getSettings);

  getSettings(): Observable<Settings> {
    this.store.dispatch(new LoadSettingsAction());
    return this.settings$;
  }

  getToken(username: string, password: string, address: string): Observable<EmbyToken> {
    const login = new EmbyLogin(username, password, address);
    return this.embyService.getEmbyToken(login);
  }

  updateSettings(settings: Settings): void {
    this.store.dispatch(new UpdateSettingsAction(settings));
  }

  searchEmby(): Observable<EmbyUdpBroadcast> {
    return this.embyService.searchEmby();
  }
}

