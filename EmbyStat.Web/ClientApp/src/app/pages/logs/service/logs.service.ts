import { Observable } from 'rxjs';

import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

import { LogFile } from '../../../shared/models/logs/log-file';

@Injectable()
export class LogService {
  private readonly baseUrl = '/api/log/';
  private getLogFilesUrl = `${this.baseUrl}list`;

  constructor(private http: HttpClient) {

  }

  getLogFiles(): Observable<LogFile[]> {
    return this.http.get<LogFile[]>(this.getLogFilesUrl);
  }
}
