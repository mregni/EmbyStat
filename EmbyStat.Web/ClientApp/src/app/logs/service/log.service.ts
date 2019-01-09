import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { Observable } from 'rxjs/Observable';
import { LogFile } from '../models/logFile';

@Injectable()
export class LogService {
  private readonly baseUrl = '/api/log/';
  private getLogFilesUrl = this.baseUrl + 'list';

  constructor(private http: HttpClient) {

  }

  getLogFiles(): Observable<LogFile[]> {
    return this.http.get<LogFile[]>(this.getLogFilesUrl);
  }
}
