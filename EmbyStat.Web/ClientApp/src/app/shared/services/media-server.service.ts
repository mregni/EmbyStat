import { Observable } from 'rxjs';

import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

import { ListContainer } from '../models/list-container';
import { MediaServerLogin } from '../models/media-server/media-server-login';
import { MediaServerPlugin } from '../models/media-server/media-server-plugin';
import { MediaServerStatus } from '../models/media-server/media-server-status';
import { MediaServerUdpBroadcast } from '../models/media-server/media-server-udp-broadcast';
import { MediaServerUser } from '../models/media-server/media-server-user';
import { ServerInfo } from '../models/media-server/server-info';
import { UserId } from '../models/media-server/user-id';
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
export class MediaServerService {
  private readonly baseUrl = '/api/mediaserver';
  private searchEmbyUrl = `${this.baseUrl}/server/search`;
  private getTestApiKeyUrl = `${ this.baseUrl }/server/test`;
  private getEmbyServerInfoUrl = `${this.baseUrl}/server/info`;
  private getEmbyStatusUrl = `${ this.baseUrl }/server/status`;
  private pingEmbyurl = `${this.baseUrl}/server/ping`;
  private getPluginsUrl = `${ this.baseUrl }/plugins`;
  private getEmbyUsersUrl = `${this.baseUrl}/users`;
  private getEmbyUserIdsUrl = `${ this.baseUrl }/ids`;

  constructor(private readonly http: HttpClient) { }

  getPlugins(): Observable<MediaServerPlugin[]> {
    return this.http.get<MediaServerPlugin[]>(this.getPluginsUrl);
  }

  getEmbyServerInfo(): Observable<ServerInfo> {
    return this.http.get<ServerInfo>(this.getEmbyServerInfoUrl);
  }

  testApiKey(login: MediaServerLogin): Observable<boolean> {
    return this.http.post<boolean>(this.getTestApiKeyUrl, login);
  }

  searchMediaServer(type: number): Observable<MediaServerUdpBroadcast> {
    return this.http.get<MediaServerUdpBroadcast>(this.searchEmbyUrl + `?serverType=${type}`);
  }

  getEmbyStatus(): Observable<MediaServerStatus> {
    return this.http.get<MediaServerStatus>(this.getEmbyStatusUrl);
  }

  getUsers(): Observable<MediaServerUser[]> {
    return this.http.get<MediaServerUser[]>(this.getEmbyUsersUrl);
  }

  getUserById(id: string): Observable<MediaServerUser> {
    return this.http.get<MediaServerUser>(`${this.getEmbyUsersUrl}/${id}`);
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
