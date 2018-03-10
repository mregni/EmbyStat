/// <reference path="../../configuration/models/configuration.ts" />
/// <reference path="../../configuration/models/configuration.ts" />
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { Store, Action } from '@ngrx/store';
import { Actions, Effect } from '@ngrx/effects';
import { map, switchMap, catchError } from 'rxjs/operators';

import 'rxjs/add/observable/throw';

import { Configuration } from '../../configuration/models/configuration';
import { EmbyUdpBroadcast } from '../../configuration/models/embyUdpBroadcast';
import { ConfigurationService } from '../../configuration/service/configuration.service';

import { ConfigurationQuery } from './configuration.reducer';
import {
  ConfigurationActionTypes,
  LoadConfigurationAction,
  LoadConfigurationSuccessAction
} from './configuration.actions';

import { ApplicationState } from '../../states/app.state';
import { EffectError } from '../../states/app.actions';

@Injectable()
export class ConfigurationFacade {
  constructor(
    private actions$: Actions,
    private store: Store<ApplicationState>,
    private configurationService: ConfigurationService
  ) { }

  configuration$ = this.store.select(ConfigurationQuery.getConfiguration);

  @Effect()
  getConfiguration$ = this.actions$
    .ofType(ConfigurationActionTypes.LOAD_CONFIGURATION)
    .pipe(
      map((data: LoadConfigurationAction) => data.payload),
      switchMap((configuration: Configuration) => this.configurationService.getConfiguration()
        .pipe(
        map((configuration: Configuration) => new LoadConfigurationSuccessAction(configuration)),
        catchError((err: any, caught: Observable<Object>) => Observable.throw(new EffectError(err))))
      )
    );

  getConfiguration(): Observable<Configuration> {
    this.store.dispatch(new LoadConfigurationAction());
    return this.configuration$;
  }

  searchEmby(): Observable<EmbyUdpBroadcast> {
    return this.configurationService.searchEmby();
  }
}

