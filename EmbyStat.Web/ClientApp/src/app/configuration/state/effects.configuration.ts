import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { Actions, Effect } from '@ngrx/effects';
import { Store } from '@ngrx/store';
import { map, switchMap, catchError, withLatestFrom } from 'rxjs/operators';
import { of } from 'rxjs/observable/of';

import 'rxjs/add/observable/throw';

import { Configuration } from '../models/configuration';
import { ConfigurationService } from '../service/configuration.service';

import {
  ConfigurationActionTypes,
  LoadConfigurationAction,
  LoadConfigurationSuccessAction,
  UpdateConfigurationAction,
  UpdateConfigurationSuccessAction,
  FireSmallEmbySyncAction,
  NoNeedConfigurationAction
} from './actions.configuration';

import { ResetPluginLoadedState } from '../../plugin/state/actions.plugin';
import { ResetServerInfoLoadedState } from '../../server/state/actions.server';

import { ConfigurationQuery } from './reducer.configuration';
import { EffectError } from '../../states/app.actions';
import { ApplicationState } from '../../states/app.state';

@Injectable()
export class ConfigurationEffects {
  constructor(
    private actions$: Actions,
    private configurationService: ConfigurationService,
    private store: Store<ApplicationState>) {
  }

  public loaded$ = this.store.select(ConfigurationQuery.getLoaded);

  @Effect()
  getConfiguration$ = this.actions$
    .ofType(ConfigurationActionTypes.LOAD_CONFIGURATION)
    .pipe(
      map((data: LoadConfigurationAction) => data.payload),
      withLatestFrom(this.loaded$),
      switchMap(([_, loaded]) => {
        return loaded
          ? of(null)
          : this.configurationService.getConfiguration();
      }),
      map((configuration: Configuration | null) => {
        return configuration
          ? new LoadConfigurationSuccessAction(configuration)
          : new NoNeedConfigurationAction();
      }),
      catchError((err: any, caught: Observable<Object>) => Observable.throw(new EffectError(err)))
  );

  @Effect()
  updateConfiguration = this.actions$
    .ofType(ConfigurationActionTypes.UPDATE_CONFIGURATION)
    .pipe(
      map((data: UpdateConfigurationAction) => data.payload),
      switchMap((config: Configuration) => {
        return this.configurationService.updateConfgiguration(config);
      }),
      switchMap((configuration: Configuration | null) => {
        return [new UpdateConfigurationSuccessAction(configuration),
          new ResetPluginLoadedState(),
          new ResetServerInfoLoadedState(),
          new FireSmallEmbySyncAction()];
      }),
      catchError((err: any, caught: Observable<Object>) => Observable.throw(new EffectError(err)))
  );

  @Effect()
  fireSmallEmbySync = this.actions$
    .ofType(ConfigurationActionTypes.FIRE_SMALL_EMBY_SYNC)
    .pipe(
      map((data: FireSmallEmbySyncAction) => data.payload),
      switchMap(() => this.configurationService.fireSmallEmbyUpdate()),
      switchMap(() => {
        return [];
      })
  );
}
