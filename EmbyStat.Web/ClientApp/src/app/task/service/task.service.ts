import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { Observable } from 'rxjs/Observable';
import { Task } from '../models/task';
@Injectable()
export class TaskService {
  private readonly baseUrl = '/api/task';
  private getTasksUrl = this.baseUrl;
  private fireTaskUrl = this.baseUrl + '/fire';
  private triggersUrl = this.baseUrl + '/triggers';

  constructor(private http: HttpClient) {

  }

  getTasks(): Observable<Task[]> {
    return this.http.get<Task[]>(this.getTasksUrl);
  }

  updateTriggers(task: Task): Observable<Task[]> {
    return this.http.put<Task[]>(this.triggersUrl, task);
  }

  fireTask(id: string): Observable<void> {
    return this.http.post<void>(this.fireTaskUrl + '/' + id,  null);
  }
}
