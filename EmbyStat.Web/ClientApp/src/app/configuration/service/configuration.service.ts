import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { Observable } from 'rxjs/Observable';
import { map } from 'rxjs/operators';

import { Configuration } from '../models/configuration';
import { EmbyUdpBroadcast } from '../models/embyUdpBroadcast';
import { EmbyLogin } from '../models/embyLogin';
import { EmbyToken } from '../models/embyToken';
import { ServerInfo } from '../models/serverInfo';

@Injectable()
export class ConfigurationService {
  private readonly getConfigurationUrl: string = '/configuration';
  private readonly updateConfigurationUrl: string = '/configuration';
  private readonly searchEmbyUrl: string = '/emby/searchemby';
  private readonly getEmbyTokenUrl: string = '/emby/generatetoken';
  private readonly getServerInfoUrl: string = '/emby/getserverinfo';
  private readonly fireSmallEmbyUpdateUrl: string = '/emby/firesmallembysync';

  constructor(private http: HttpClient) {
    
  }

  getConfiguration(): Observable<Configuration> {
    return this.http.get<Configuration>('/api' + this.getConfigurationUrl);
  }

  getEmbyToken(login: EmbyLogin): Observable<EmbyToken> {
    return this.http.post<EmbyToken>('/api' + this.getEmbyTokenUrl, login);
  }

  updateConfgiguration(configuration: Configuration): Observable<Configuration> {
    console.log(configuration);
    return this.http.put<Configuration>('/api' + this.updateConfigurationUrl, configuration);
  }

  searchEmby(): Observable<EmbyUdpBroadcast> {
    return this.http.get<EmbyUdpBroadcast>('/api' + this.searchEmbyUrl);
  }

  fireSmallEmbyUpdate(): Observable<void> {
    return this.http.post<void>('/api' + this.fireSmallEmbyUpdateUrl, {});
  }

  getServerInfo(): Observable<ServerInfo> {
    return this.http.get<ServerInfo>('/api' + this.getServerInfoUrl);
  }
}
