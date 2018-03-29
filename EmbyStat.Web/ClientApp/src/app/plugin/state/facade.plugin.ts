import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { Store } from '@ngrx/store';
import { Actions } from '@ngrx/effects';

import 'rxjs/add/observable/throw';

import { EmbyPluginStore } from '../models/embyPluginStore';
import { PluginService } from '../service/plugin.service';

import { PluginQuery } from './reducer.plugin';
import { LoadPluginAction } from './actions.plugin';

import { ApplicationState } from '../../states/app.state';

@Injectable()
export class PluginFacade {
  constructor(
    private actions$: Actions,
    private store: Store<ApplicationState>,
    private pluginService: PluginService
  ) { }

  plugins$ = this.store.select(PluginQuery.getPlugins);

  getPlugins(): Observable<EmbyPluginStore> {
    this.store.dispatch(new LoadPluginAction());
    return this.plugins$;
  }
}

