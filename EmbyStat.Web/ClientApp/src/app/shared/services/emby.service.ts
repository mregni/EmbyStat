import { Observable } from 'rxjs';

import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

import { EmbyLogin } from '../models/emby/emby-login';
import { EmbyPlugin } from '../models/emby/emby-plugin';
import { EmbyStatus } from '../models/emby/emby-status';
import { EmbyToken } from '../models/emby/emby-token';
import { EmbyUdpBroadcast } from '../models/emby/emby-udp-broadcast';
import { EmbyUser } from '../models/emby/emby-user';
import { ServerInfo } from '../models/emby/server-info';
import { UserId } from '../models/emby/user-id';
import { ListContainer } from '../models/list-container';
import { UserMediaView } from '../models/session/user-media-view';

class UrlObject {
  url: string;

  constructor(url: string) {
    this.url = url;
  }
}

@Injectable({
  providedIn: 'root'
})
export class EmbyService {
  private readonly baseUrl = '/api/emby';
  private searchEmbyUrl = `${this.baseUrl}/server/search`;
  private getTestApiKeyUrl = `${ this.baseUrl }/server/test`;
  private getEmbyServerInfoUrl = `${this.baseUrl}/server/info`;
  private getEmbyStatusUrl = `${ this.baseUrl }/server/status`;
  private pingEmbyurl = `${this.baseUrl}/server/ping`;
  private getPluginsUrl = `${ this.baseUrl }/plugins`;
  private getEmbyUsersUrl = `${this.baseUrl}/users`;
  private getEmbyUserIdsUrl = `${ this.baseUrl }/ids`;

  constructor(private readonly http: HttpClient) { }

  getPlugins(): Observable<EmbyPlugin[]> {
    return this.http.get<EmbyPlugin[]>(this.getPluginsUrl);
  }

  getEmbyServerInfo(): Observable<ServerInfo> {
    return this.http.get<ServerInfo>(this.getEmbyServerInfoUrl);
  }

  testApiKey(login: EmbyLogin): Observable<boolean> {
    return this.http.post<boolean>(this.getTestApiKeyUrl, login);
  }

  searchEmby(): Observable<EmbyUdpBroadcast> {
    return this.http.get<EmbyUdpBroadcast>(this.searchEmbyUrl);
  }

  getEmbyStatus(): Observable<EmbyStatus> {
    return this.http.get<EmbyStatus>(this.getEmbyStatusUrl);
  }

  getUsers(): Observable<EmbyUser[]> {
    return this.http.get<EmbyUser[]>(this.getEmbyUsersUrl);
  }

  getUserById(id: string): Observable<EmbyUser> {
    return this.http.get<EmbyUser>(`${this.getEmbyUsersUrl}/${id}`);
  }

  getUserIdList(): Observable<UserId[]> {
    return this.http.get<UserId[]>(this.getEmbyUserIdsUrl);
  }

  getUserViewsByUserId(id: string, page: number, size: number): Observable<ListContainer<UserMediaView>> {
    return this.http.get<ListContainer<UserMediaView>>(`${this.getEmbyUsersUrl}/${id}/views/${page}/${size}`);
  }

  pingEmby(url: string): Observable<boolean> {
    const urlObj = new UrlObject(url);
    return this.http.post<boolean>(this.pingEmbyurl, urlObj);
  }
}
