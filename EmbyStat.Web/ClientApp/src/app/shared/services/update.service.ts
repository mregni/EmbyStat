import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs/Observable';

import { UpdateResult } from '../models/updateResult';

@Injectable()
export class UpdateService {
  private readonly baseUrl: string = '/api/update/';
  private readonly checkForUpdateUrl: string = this.baseUrl + 'checkforupdate';
  private readonly startUpdateUrl: string = this.baseUrl + 'startupdate';

  constructor(private http: HttpClient) { }

  checkForUpdate(): Observable<UpdateResult> {
    return this.http.get<UpdateResult>(this.checkForUpdateUrl);
  }

  checkAndStartUpdate(): Observable<void> {
    return this.http.post<void>(this.startUpdateUrl, {});
  }
}
