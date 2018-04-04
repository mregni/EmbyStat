import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { Observable } from 'rxjs/Observable';
import { Task } from '../models/task';
import { TaskStatus } from '../models/taskStatus';

@Injectable()
export class TaskService {
  private readonly getTasksUrl: string = '/task';
  private readonly fireTaskUrl: string = '/task/fire';
  private readonly getStatesUrl: string = '/task/state';
  private readonly triggersUrl: string = '/trigger';

  constructor(private http: HttpClient) {

  }

  getTasks(): Observable<Task[]> {
    return this.http.get<Task[]>('/api' + this.getTasksUrl);
  }

  getStates(): Observable<TaskStatus[]> {
    return this.http.get<TaskStatus[]>('/api' + this.getStatesUrl);
  }

  getStateForId(id: string): Observable<TaskStatus> {
    return this.http.get<TaskStatus>('/api' + this.getStatesUrl + '/' + id);
  }

  fireTask(id: string): Observable<void> {
    return this.http.post<void>('/api' + this.fireTaskUrl, { id: id });
  }
}
