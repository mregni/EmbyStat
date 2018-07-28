import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { Store } from '@ngrx/store';

import 'rxjs/add/observable/throw';

import { EmbyPlugin } from '../models/embyPlugin';
import { PluginService } from '../service/plugin.service';

@Injectable()
export class PluginFacade {
  constructor(private pluginService: PluginService ) { }

  getPlugins(): Observable<EmbyPlugin[]> {
    return this.pluginService.getPlugins();
  }
}

