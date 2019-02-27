import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { EmbyUdpBroadcast } from '../models/emby/emby-udp-broadcast';
import { EmbyLogin } from '../models/emby//emby-login';
import { EmbyToken } from '../models/emby/emby-token';
import { EmbyStatus } from '../models/emby/emby-status';
import { ServerInfo } from '../models/emby/server-info';
import { EmbyPlugin } from '../models/emby/emby-plugin';
import { EmbyUser } from '../models/emby/emby-user';
import { UserMediaView } from '../models/session/user-media-view';

@Injectable()
export class EmbyService {
  private readonly baseUrl = '/api/emby';
  private searchEmbyUrl = this.baseUrl + '/server/search';
  private getEmbyTokenUrl = this.baseUrl + '/server/token';
  private getServerInfoUrl = this.baseUrl + '/server/info';
  private getEmbyStatusUrl = this.baseUrl + '/server/status';
  private getPluginsUrl = this.baseUrl + '/plugins';
  private getEmbyUsersUrl = this.baseUrl + '/users';
  constructor(private http: HttpClient) { }

  getEmbyToken(login: EmbyLogin): Observable<EmbyToken> {
    return this.http.post<EmbyToken>(this.getEmbyTokenUrl, login);
  }

  searchEmby(): Observable<EmbyUdpBroadcast> {
    return this.http.get<EmbyUdpBroadcast>(this.searchEmbyUrl);
  }

  getEmbyStatus(): Observable<EmbyStatus> {
    return this.http.get<EmbyStatus>(this.getEmbyStatusUrl);
  }

  getServerInfo(): Observable<ServerInfo> {
    return this.http.get<ServerInfo>(this.getServerInfoUrl);
  }

  getPlugins(): Observable<EmbyPlugin[]> {
    return this.http.get<EmbyPlugin[]>(this.getPluginsUrl);
  }

  getUsers(): Observable<EmbyUser[]> {
    return this.http.get<EmbyUser[]>(this.getEmbyUsersUrl);
  }

  getUserById(id: string): Observable<EmbyUser> {
    return this.http.get<EmbyUser>(this.getEmbyUsersUrl + '/' + id);
  }

  getUserViewsByUserId(id: string): Observable<UserMediaView[]> {
    return this.http.get<UserMediaView[]>(this.getEmbyUsersUrl + '/' + id + "/views");
  }
}
