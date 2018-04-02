import { Component, OnInit } from '@angular/core';
import { TaskFacade } from './state/facade.task';
import { Observable } from 'rxjs/Observable';
import { Task } from './models/Task';

@Component({
  selector: 'app-task',
  templateUrl: './task.component.html',
  styleUrls: ['./task.component.scss']
})
export class TaskComponent implements OnInit {

  public tasks$: Observable<Task[]>;
  constructor(private taskFacade: TaskFacade) { }

  public fireTask(id: string): void {
    this.taskFacade.fireTask(id);
  }

  ngOnInit() {
    this.tasks$ = this.taskFacade.getTasks();
  }
}
