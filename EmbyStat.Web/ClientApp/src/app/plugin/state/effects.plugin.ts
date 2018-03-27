import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { Actions, Effect } from '@ngrx/effects';
import { map, switchMap, catchError } from 'rxjs/operators';

import 'rxjs/add/observable/throw';

import { EmbyPlugin } from '../models/embyPlugin';
import { PluginService } from '../service/plugin.service';

import { PluginActionTypes, LoadPluginAction, LoadPluginSuccessAction } from './actions.plugin';

import { EffectError } from '../../states/app.actions';

@Injectable()
export class PluginEffects {
  constructor(
    private actions$: Actions,
    private pluginService: PluginService) {
  }

  @Effect()
  getPlugins$ = this.actions$
    .ofType(PluginActionTypes.LOAD_PLUGINS)
    .pipe(
      map((data: LoadPluginAction) => data.payload),
      switchMap(() => this.pluginService.getPlugins()
        .pipe(
        map((plugins: EmbyPlugin[]) => new LoadPluginSuccessAction(plugins)),
          catchError((err: any, caught: Observable<Object>) => Observable.throw(new EffectError(err))))
      )
    );
}
