import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs/Observable';

import { UpdateResult } from '../models/update-result';

@Injectable()
export class SystemService {
  private readonly baseUrl: string = '/api/system/';
  private checkForUpdateUrl: string = this.baseUrl + 'checkforupdate';
  private startUpdateUrl: string = this.baseUrl + 'startupdate';
  private pingUrl: string = this.baseUrl + 'ping';

  constructor(private http: HttpClient) { }

  checkForUpdate(): Observable<UpdateResult> {
    return this.http.get<UpdateResult>(this.checkForUpdateUrl);
  }

  checkAndStartUpdate(): Observable<boolean> {
    return this.http.post<boolean>(this.startUpdateUrl, {});
  }

  ping(): Observable<string> {
    return this.http.get<string>(this.pingUrl);
  }
}
