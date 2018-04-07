import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';

import { TaskService } from '../service/task.service';

import { Task } from '../models/task';

@Injectable()
export class TaskFacade {
  constructor(private taskService: TaskService) { }

  public tasks$: Observable<Task[]>;

  getTasks(): Observable<Task[]> {
    this.tasks$ = this.taskService.getTasks();
    return this.tasks$;
  }

  updateTrigger(task: Task): Observable<Task[]> {
    this.tasks$ = this.taskService.updateTriggers(task);
    return this.tasks$;
  }

  fireTask(id: string): void {
    this.taskService.fireTask(id).subscribe();
  }
}

