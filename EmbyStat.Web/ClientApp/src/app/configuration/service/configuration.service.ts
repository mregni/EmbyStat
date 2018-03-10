import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { Observable } from 'rxjs/Observable';
import { map } from 'rxjs/operators';

import { Configuration } from '../models/configuration';
import { EmbyUdpBroadcast } from '../models/embyUdpBroadcast';

interface ConfigurationResponse { item: Configuration }

@Injectable()
export class ConfigurationService {
  private readonly getConfigurationUrl: string = '/configuration';
  private readonly searchEmbyUrl: string = '/configuration/searchemby';

  constructor(private http: HttpClient) {
    
  }

  getConfiguration(): Observable<Configuration> {
    return this.http
      .get<Configuration>('/api' + this.getConfigurationUrl)
      .pipe(map(data => data));
  }

  searchEmby(): Observable<EmbyUdpBroadcast> {
    return this.http
      .get<EmbyUdpBroadcast>('/api' + this.searchEmbyUrl)
      .pipe(map(data => data));
  }
}
