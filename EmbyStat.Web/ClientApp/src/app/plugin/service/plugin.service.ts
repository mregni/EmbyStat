import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { Observable } from 'rxjs/Observable';
import { EmbyPlugin } from '../models/embyPlugin';

@Injectable()
export class PluginService {
  private readonly getPluginsUrl: string = '/plugin';

  constructor(private http: HttpClient) {

  }

  getPlugins(): Observable<EmbyPlugin[]> {
    return this.http.get<EmbyPlugin[]>('/api' + this.getPluginsUrl);
  }
}
