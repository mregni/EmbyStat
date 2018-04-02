import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';

import { TaskService } from '../service/task.service';

import { Task } from '../models/task';

@Injectable()
export class TaskFacade {
  constructor(private taskService: TaskService) { }

  getTasks(): Observable<Task[]> {
    return this.taskService.getTasks();
  }

  fireTask(id: string): void {
    console.log(id);
    this.taskService.fireTask(id).subscribe();
  }
}

