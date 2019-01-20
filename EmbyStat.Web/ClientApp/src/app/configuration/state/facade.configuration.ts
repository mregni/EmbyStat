import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { Store } from '@ngrx/store';

import 'rxjs/add/observable/throw';

import { Configuration } from '../models/configuration';
import { EmbyUdpBroadcast } from '../../shared/models/emby/emby-udp-broadcast';
import { EmbyToken } from '../../shared/models/emby/emby-token';
import { EmbyLogin } from '../../shared/models/emby/emby-login';
import { ConfigurationService } from '../service/configuration.service';
import { EmbyService } from '../../shared/services/emby.service';

import { ConfigurationQuery } from './reducer.configuration';
import { LoadConfigurationAction, UpdateConfigurationAction } from './actions.configuration';

import { ApplicationState } from '../../states/app.state';

@Injectable()
export class ConfigurationFacade {
  constructor(
    private store: Store<ApplicationState>,
    private configurationService: ConfigurationService,
    private embyService: EmbyService
  ) { }

  configuration$ = this.store.select(ConfigurationQuery.getConfiguration);

  getConfiguration(): Observable<Configuration> {
    this.store.dispatch(new LoadConfigurationAction());
    return this.configuration$;
  }

  getToken(username: string, password: string, address: string): Observable<EmbyToken> {
    const login = new EmbyLogin(username, password, address);
    return this.embyService.getEmbyToken(login);
  }

  updateConfiguration(config: Configuration): void {
    this.store.dispatch(new UpdateConfigurationAction(config));
  }

  searchEmby(): Observable<EmbyUdpBroadcast> {
    return this.embyService.searchEmby();
  }
}

