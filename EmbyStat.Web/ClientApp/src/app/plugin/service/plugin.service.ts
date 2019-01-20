import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { Observable } from 'rxjs/Observable';
import { EmbyPlugin } from '../../shared/models/emby/emby-plugin';

@Injectable()
export class PluginService {
  private readonly baseUrl = '/api/plugin';

  constructor(private http: HttpClient) {

  }

  getPlugins(): Observable<EmbyPlugin[]> {
    return this.http.get<EmbyPlugin[]>(this.baseUrl);
  }
}
