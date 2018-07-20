import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs/Observable';

import { EmbyUdpBroadcast } from '../models/emby/embyUdpBroadcast';
import { EmbyLogin } from '../models/emby//embyLogin';
import { EmbyToken } from '../models/emby/embyToken';
import { EmbyStatus } from '../models/emby/embyStatus';

@Injectable()
export class EmbyService {
  private readonly searchEmbyUrl: string = '/emby/searchemby';
  private readonly getEmbyTokenUrl: string = '/emby/generatetoken';
  private readonly getServerInfoUrl: string = '/emby/getserverinfo';
  private readonly fireSmallEmbyUpdateUrl: string = '/emby/firesmallembysync';
  private readonly getEmbyStatusUrl: string = '/emby/getembystatus';

  constructor(private http: HttpClient) { }

  getEmbyToken(login: EmbyLogin): Observable<EmbyToken> {
    return this.http.post<EmbyToken>('/api' + this.getEmbyTokenUrl, login);
  }

  searchEmby(): Observable<EmbyUdpBroadcast> {
    return this.http.get<EmbyUdpBroadcast>('/api' + this.searchEmbyUrl);
  }

  fireSmallEmbyUpdate(): Observable<void> {
    return this.http.post<void>('/api' + this.fireSmallEmbyUpdateUrl, {});
  }

  getEmbyStatus(): Observable<EmbyStatus> {
    return this.http.get<EmbyStatus>('/api' + this.getEmbyStatusUrl);
  }
}
