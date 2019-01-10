import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { Observable } from 'rxjs/Observable';
import { Task } from '../models/task';
@Injectable()
export class TaskService {
  private readonly baseUrl = '/api/task';
  private firePingTaskUrl = this.baseUrl + '/fire/ping';

  constructor(private http: HttpClient) {

  }

  firePingTask(): Observable<void> {
    return this.http.post<void>(this.firePingTaskUrl, null);
  }
}
