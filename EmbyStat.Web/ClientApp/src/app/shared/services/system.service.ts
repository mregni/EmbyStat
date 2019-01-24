import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs/Observable';

import { UpdateResult } from '../models/update-result';
import { Ping } from '../models/ping';

@Injectable()
export class SystemService {
  private readonly baseUrl = '/api/system/';
  private checkForUpdateUrl = this.baseUrl + 'checkforupdate';
  private startUpdateUrl = this.baseUrl + 'startupdate';
  private pingUrl = this.baseUrl + 'ping';

  constructor(private http: HttpClient) { }

  checkForUpdate(): Observable<UpdateResult> {
    return this.http.get<UpdateResult>(this.checkForUpdateUrl);
  }

  checkAndStartUpdate(): Observable<boolean> {
    return this.http.post<boolean>(this.startUpdateUrl, {});
  }

  ping(): Observable<void> {
    const headers = new HttpHeaders({
      'Cache-Control': 'no-cache',
      'Pragma': 'no-cache'
    });
    return this.http.get<void>(this.pingUrl, { headers: headers});
  }
}
