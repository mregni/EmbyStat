import { Observable, of, throwError } from 'rxjs';
import { catchError, map, switchMap, withLatestFrom } from 'rxjs/operators';

import { Injectable } from '@angular/core';
import { Actions, Effect, ofType } from '@ngrx/effects';
import { Store } from '@ngrx/store';

import { Settings } from '../../../shared/models/settings/settings';
import { EmbyService } from '../../../shared/services/emby.service';
import { SettingsService } from '../../../shared/services/settings.service';
import { EffectError } from '../../../states/app.actions';
import { ApplicationState } from '../../../states/app.state';
import {
    LoadSettingsAction, LoadSettingsSuccessAction, NoNeedSettingsAction, SettingsActionTypes,
    UpdateSettingsAction, UpdateSettingsSuccessAction
} from './settings.actions';
import { SettingsQuery } from './settings.reducer';

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
        console.log(settings);
        return this.settingsService.updateSettings(settings);
      }),
      switchMap((settings: Settings | null) => {
        console.log(settings);
        return [new UpdateSettingsSuccessAction(settings)];
      }),
      catchError((err: any, caught: Observable<Object>) => throwError(new EffectError(err)))
    );
}
