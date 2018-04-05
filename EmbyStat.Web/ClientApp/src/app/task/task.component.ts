import { Component, OnInit, OnDestroy } from '@angular/core';
import { TaskFacade } from './state/facade.task';
import { Observable } from 'rxjs/Observable';
import { Subscription } from 'rxjs/Subscription';
import { Task } from './models/Task';
import { TaskStatus } from './models/taskStatus';
import { HubConnection } from '@aspnet/signalr';

import 'rxjs/Rx';

@Component({
  selector: 'app-task',
  templateUrl: './task.component.html',
  styleUrls: ['./task.component.scss']
})
export class TaskComponent implements OnInit, OnDestroy {
  private hubConnection: HubConnection;
  public tasks: Task[];

  constructor(private taskFacade: TaskFacade) {
    this.hubConnection = new HubConnection('/tasksignal');

    this.hubConnection.on('ReceiveInfo', (data: Task[]) => {
      this.tasks = data;
      console.log(data);
    });
  }


  ngOnInit() {
    this.taskFacade.getTasks().subscribe(data => this.tasks = data);
    this.hubConnection
      .start()
      .then(() => {
        console.log('Connection started!');
      })
      .catch(err => console.log('Error while establishing connection :('));
  }

  public fireTask(id: string): void {
    this.taskFacade.fireTask(id);
  }

  ngOnDestroy() {
    if (this.hubConnection !== undefined) {
      this.hubConnection.stop();
    }
  }
}
