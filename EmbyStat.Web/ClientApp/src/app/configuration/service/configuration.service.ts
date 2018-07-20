import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { Observable } from 'rxjs/Observable';

import { Configuration } from '../models/configuration';

@Injectable()
export class ConfigurationService {
  private readonly getConfigurationUrl: string = '/configuration';
  private readonly updateConfigurationUrl: string = '/configuration';

  constructor(private http: HttpClient) {

  }

  getConfiguration(): Observable<Configuration> {
    return this.http.get<Configuration>('/api' + this.getConfigurationUrl);
  }

  updateConfgiguration(configuration: Configuration): Observable<Configuration> {
    return this.http.put<Configuration>('/api' + this.updateConfigurationUrl, configuration);
  }
}
