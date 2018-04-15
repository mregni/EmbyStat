import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { Store } from '@ngrx/store';

import 'rxjs/add/observable/throw';

import { EmbyPlugin } from '../models/embyPlugin';
import { PluginService } from '../service/plugin.service';

import { PluginQuery } from './reducer.plugin';
import { LoadPluginAction } from './actions.plugin';

import { ApplicationState } from '../../states/app.state';

@Injectable()
export class PluginFacade {
  constructor(
    private store: Store<ApplicationState>
  ) { }

  plugins$ = this.store.select(PluginQuery.getPlugins);

  getPlugins(): Observable<EmbyPlugin[]> {
    this.store.dispatch(new LoadPluginAction());
    return this.plugins$;
  }
}

