import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ResponseContentType } from '@angular/http';

import { Observable } from 'rxjs/Observable';
import { LogFile } from '../models/logFile';

@Injectable()
export class LogService {
  private readonly getLogFilesUrl: string = '/log/list';
  private readonly downloadLogUrl: string = '/log/download';

  constructor(private http: HttpClient) {

  }

  getLogFiles(): Observable<LogFile[]> {
    return this.http.get<LogFile[]>('/api' + this.getLogFilesUrl);
  }
}
