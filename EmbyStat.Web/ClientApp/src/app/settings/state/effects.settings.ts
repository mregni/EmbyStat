import { throwError, Observable, of } from 'rxjs';
import { Injectable } from '@angular/core';
import { Actions, Effect, ofType } from '@ngrx/effects';
import { Store } from '@ngrx/store';
import { map, switchMap, catchError, withLatestFrom } from 'rxjs/operators';



import { Settings } from '../models/settings';
import { SettingsService } from '../settings.service';
import { EmbyService } from '../../shared/services/emby.service';

import {
  SettingsActionTypes,
  LoadSettingsAction,
  LoadSettingsSuccessAction,
  UpdateSettingsAction,
  UpdateSettingsSuccessAction,
  NoNeedSettingsAction
} from './actions.settings';

import { ResetServerInfoLoadedState } from '../../server/state/actions.server';

import { SettingsQuery } from './reducer.settings';
import { EffectError } from '../../states/app.actions';
import { ApplicationState } from '../../states/app.state';

@Injectable()
export class SettingsEffects {
  constructor(
    private actions$: Actions,
    private settingsService: SettingsService,
    private embyService: EmbyService,
    private store: Store<ApplicationState>) {
  }

  loaded$ = this.store.select(SettingsQuery.getLoaded);

  @Effect()
  getConfiguration$ = this.actions$
    .pipe(
      ofType(SettingsActionTypes.LOAD_SETTINGS),
      map((data: LoadSettingsAction) => data.payload),
      withLatestFrom(this.loaded$),
      switchMap(([_, loaded]) => {
        return loaded
          ? of(null)
          : this.settingsService.getSettings();
      }),
      map((settings: Settings | null) => {
        return settings
          ? new LoadSettingsSuccessAction(settings)
          : new NoNeedSettingsAction();
      }),
      catchError((err: any, caught: Observable<Object>) => throwError(new EffectError(err)))
    );

  @Effect()
  updateConfiguration = this.actions$
    .pipe(
      ofType(SettingsActionTypes.UPDATE_SETTINGS),
      map((data: UpdateSettingsAction) => data.payload),
      switchMap((settings: Settings) => {
        return this.settingsService.updateSettings(settings);
      }),
      switchMap((settings: Settings | null) => {
        return [new UpdateSettingsSuccessAction(settings),
        new ResetServerInfoLoadedState()];
      }),
      catchError((err: any, caught: Observable<Object>) => throwError(new EffectError(err)))
    );
}
