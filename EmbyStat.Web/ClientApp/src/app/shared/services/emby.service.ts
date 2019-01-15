import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs/Observable';

import { EmbyUdpBroadcast } from '../models/emby/emby-udp-broadcast';
import { EmbyLogin } from '../models/emby//emby-login';
import { EmbyToken } from '../models/emby/emby-token';
import { EmbyStatus } from '../models/emby/emby-status';
import { ServerInfo } from '../models/emby/server-info';

@Injectable()
export class EmbyService {
  private readonly baseUrl = '/api/emby/';
  private searchEmbyUrl = this.baseUrl + 'searchemby';
  private getEmbyTokenUrl = this.baseUrl + 'generatetoken';
  private getServerInfoUrl = this.baseUrl + 'getserverinfo';
  private fireSmallEmbyUpdateUrl = this.baseUrl + 'firesmallembysync';
  private getEmbyStatusUrl = this.baseUrl + 'getembystatus';

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
}
