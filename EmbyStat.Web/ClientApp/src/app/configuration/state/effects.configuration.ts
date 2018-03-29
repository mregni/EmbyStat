import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { Actions, Effect } from '@ngrx/effects';
import { map, switchMap, catchError } from 'rxjs/operators';

import 'rxjs/add/observable/throw';

import { Configuration } from '../models/configuration';
import { ConfigurationService } from '../service/configuration.service';

import {
  ConfigurationActionTypes,
  LoadConfigurationAction,
  LoadConfigurationSuccessAction,
  UpdateConfigurationAction,
  UpdateConfigurationSuccessAction,
  FireSmallEmbySyncAction
} from './actions.configuration';

import { LoadPluginAction } from '../../plugin/state/actions.plugin'

import { EffectError } from '../../states/app.actions';

@Injectable()
export class ConfigurationEffects {
  constructor(
    private actions$: Actions,
    private configurationService: ConfigurationService) {
  }

  @Effect()
  getConfiguration$ = this.actions$
    .ofType(ConfigurationActionTypes.LOAD_CONFIGURATION)
    .pipe(
      map((data: LoadConfigurationAction) => data.payload),
      switchMap(() => this.configurationService.getConfiguration()
        .pipe(
          map((configuration: Configuration) => new LoadConfigurationSuccessAction(configuration)),
          catchError((err: any, caught: Observable<Object>) => Observable.throw(new EffectError(err))))
      )
  );

  @Effect()
  updateConfiguration = this.actions$
    .ofType(ConfigurationActionTypes.UPDATE_CONFIGURATION)
      .pipe(
        map((data: UpdateConfigurationAction) => data.payload),
        switchMap((config: Configuration) => this.configurationService.updateConfgiguration(config)
          .pipe(
            map((configuration: Configuration) => new UpdateConfigurationSuccessAction(configuration)),
            catchError((err: any, caught: Observable<Object>) => Observable.throw(new EffectError(err))))
          .pipe(
            map((configuration: Configuration) => new FireSmallEmbySyncAction()),
            catchError((err: any, caught: Observable<Object>) => Observable.throw(new EffectError(err))))
        )
  );

  @Effect()
  fireSmallEmbySync = this.actions$
    .ofType(ConfigurationActionTypes.FIRE_SMALL_EMBY_SYNC)
      .pipe(
        map((data: FireSmallEmbySyncAction) => data.payload),
        switchMap(() => this.configurationService.fireSmallEmbyUpdate()
          .pipe(
            map(() => new LoadPluginAction()),
            catchError((err: any, caught: Observable<Object>) => Observable.throw(new EffectError(err))))
        )
      );
}
