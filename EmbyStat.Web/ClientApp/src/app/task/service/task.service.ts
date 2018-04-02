import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { Observable } from 'rxjs/Observable';
import { Task } from '../models/task';

@Injectable()
export class TaskService {
  private readonly getTasksUrl: string = '/task';
  private readonly fireTaskUrl: string = '/task/fire';

  constructor(private http: HttpClient) {

  }

  getTasks(): Observable<Task[]> {
    return this.http.get<Task[]>('/api' + this.getTasksUrl);
  }

  fireTask(id: string): Observable<void> {
    return this.http.post<void>('/api' + this.fireTaskUrl, { id: id });
  }
}
