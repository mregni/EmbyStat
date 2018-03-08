/// <reference path="../../configuration/models/configuration.ts" />
/// <reference path="../../configuration/models/configuration.ts" />
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { Store, Action } from '@ngrx/store';
import { Actions, Effect } from '@ngrx/effects';
import { map, switchMap } from 'rxjs/operators';

import { Configuration } from '../../configuration/models/configuration';
import { ConfigurationService } from '../../configuration/service/configuration.service';

import { ConfigurationQuery } from './configuration.reducer';
import {
  ConfigurationActionTypes,
  LoadConfigurationAction,
  LoadConfigurationSuccessAction
} from './configuration.actions';
import { NoopAction } from '../app.actions';

import { ApplicationState } from '../app.state';


@Injectable()
export class ConfigurationFacade {
  constructor(
    private actions$: Actions,
    private store: Store<ApplicationState>,
    private configurationService: ConfigurationService
  ) { }

  configuration$ = this.store.select(ConfigurationQuery.getConfiguration);

  @Effect() getConfiguration$ = this.actions$
    .ofType(ConfigurationActionTypes.LOAD_CONFIGURATION)
    .pipe(
    map((data: LoadConfigurationAction) => data.payload),
    switchMap((configuration: Configuration) => this.configurationService.getConfiguration()),
    map((configuration: Configuration) => {
      console.log(configuration);
        return configuration ? new LoadConfigurationSuccessAction(configuration) : new NoopAction();
      })
    );

  getConfiguration(): Observable<Configuration> {
    this.store.dispatch(new LoadConfigurationAction());
    return this.configuration$;
  }
}

