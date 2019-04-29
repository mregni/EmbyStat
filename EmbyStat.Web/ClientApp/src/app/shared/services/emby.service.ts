import { Observable } from 'rxjs';

import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

import { EmbyPlugin } from '../models/emby/emby-plugin';
import { ServerInfo } from '../models/emby/server-info';

@Injectable({
  providedIn: 'root'
})
export class EmbyService {
  private readonly baseUrl = '/api/emby';
  private getPluginsUrl = this.baseUrl + '/plugins';
  private getEmbyServerInfoUrl = this.baseUrl + '/server/info';

  constructor(private readonly http: HttpClient) { }

  getPlugins(): Observable<EmbyPlugin[]> {
    return this.http.get<EmbyPlugin[]>(this.getPluginsUrl);
  }

  getEmbyServerInfo(): Observable<ServerInfo> {
    return this.http.get<ServerInfo>(this.getEmbyServerInfoUrl);
  }
}
