import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { Store } from '@ngrx/store';
import { Actions } from '@ngrx/effects';

import 'rxjs/add/observable/throw';

import { Configuration } from '../models/configuration';
import { EmbyUdpBroadcast } from '../models/embyUdpBroadcast';
import { EmbyToken } from '../models/embyToken';
import { EmbyLogin } from '../models/embyLogin';
import { ConfigurationService } from '../service/configuration.service';

import { ConfigurationQuery } from './reducer.configuration';
import { LoadConfigurationAction, UpdateConfigurationAction, FireSmallEmbySyncAction } from './actions.configuration';

import { ApplicationState } from '../../states/app.state';

@Injectable()
export class ConfigurationFacade {
  constructor(
    private actions$: Actions,
    private store: Store<ApplicationState>,
    private configurationService: ConfigurationService
  ) { }

  configuration$ = this.store.select(ConfigurationQuery.getConfiguration);

  getConfiguration(): Observable<Configuration> {
    this.store.dispatch(new LoadConfigurationAction());
    return this.configuration$;
  }

  getToken(username: string, password: string, address: string): Observable<EmbyToken> {
    const login = new EmbyLogin(username, password, address);
    return this.configurationService.getEmbyToken(login);
  }

  updateConfiguration(config: Configuration): void {
    this.store.dispatch(new UpdateConfigurationAction(config));
  }

  searchEmby(): Observable<EmbyUdpBroadcast> {
    return this.configurationService.searchEmby();
  }

  fireSmallEmbySync(): void {
    this.store.dispatch(new FireSmallEmbySyncAction());
  }
}

