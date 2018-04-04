import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';

import { TaskService } from '../service/task.service';

import { Task } from '../models/task';
import { TaskStatus } from '../models/taskStatus';

@Injectable()
export class TaskFacade {
  constructor(private taskService: TaskService) { }

  public tasks$: Observable<Task[]>;

  getTasks(): Observable<Task[]> {
    this.tasks$ = this.taskService.getTasks();
    return this.tasks$;
  }

  pollTaskStates(): Observable<TaskStatus[]> {
    return this.taskService.getStates();
  }

  pollTaskState(id: string): Observable<TaskStatus> {
    return this.taskService.getStateForId(id);
  }

  fireTask(id: string): void {
    this.taskService.fireTask(id).subscribe();
  }
}

