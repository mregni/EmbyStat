import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { Observable } from 'rxjs/Observable';
import { ServerInfo } from '../models/serverInfo';

@Injectable()
export class ServerService {
  private readonly getServerInfoUrl: string = '/emby/getserverinfo';

  constructor(private http: HttpClient) {

  }

  getServerInfo(): Observable<ServerInfo> {
    return this.http.get<ServerInfo>('/api' + this.getServerInfoUrl);
  }
}
