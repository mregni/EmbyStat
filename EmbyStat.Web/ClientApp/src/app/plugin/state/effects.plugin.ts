import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { Actions, Effect } from '@ngrx/effects';
import { Store } from '@ngrx/store';
import { map, switchMap, catchError, withLatestFrom } from 'rxjs/operators';
import { of } from 'rxjs/observable/of';

import 'rxjs/add/observable/throw';

import { EmbyPlugin } from '../models/embyPlugin';
import { PluginService } from '../service/plugin.service';

import { PluginActionTypes, LoadPluginAction, LoadPluginSuccessAction, NoNeedPluginAction } from './actions.plugin';

import { PluginQuery } from './reducer.plugin';
import { EffectError } from '../../states/app.actions';
import { ApplicationState } from '../../states/app.state';

@Injectable()
export class PluginEffects {
  constructor(
    private actions$: Actions,
    private pluginService: PluginService,
    private store: Store<ApplicationState>) {
  }

  public loaded$ = this.store.select(PluginQuery.getLoaded);

  @Effect()
  getPlugins$ = this.actions$
    .ofType(PluginActionTypes.LOAD_PLUGINS)
    .pipe(
      map((data: LoadPluginAction) => data.payload),
      withLatestFrom(this.loaded$),
      switchMap(([_, loaded]) => {
        return loaded
          ? of(null)
          : this.pluginService.getPlugins();
      }),
      map((plugins: EmbyPlugin[] | null) => {
        return plugins
          ? new LoadPluginSuccessAction(plugins)
          : new NoNeedPluginAction();
      }),
      catchError((err: any, caught: Observable<Object>) => Observable.throw(new EffectError(err)))
    );
}
