import { Component, OnInit, OnDestroy } from '@angular/core';
import { TaskFacade } from './state/facade.task';
import { Observable } from 'rxjs/Observable';
import { Subscription } from 'rxjs/Subscription';
import { Task } from './models/Task';
import { TaskStatus } from './models/taskStatus';

import 'rxjs/Rx';

@Component({
  selector: 'app-task',
  templateUrl: './task.component.html',
  styleUrls: ['./task.component.scss']
})
export class TaskComponent implements OnInit, OnDestroy {
  private statePollingSub: Subscription;
  public tasks$: Observable<Task[]>;
  public pollingQueue: string[] = [];

  constructor(private taskFacade: TaskFacade) { }

  public fireTask(id: string): void {
    this.taskFacade.fireTask(id);
  }

  ngOnInit() {
    this.tasks$ = this.taskFacade.getTasks();

    this.statePollingSub = Observable
      .interval(5000)
      .startWith(0)
      .switchMap(() => this.taskFacade.pollTaskStates())
      .map((data) => {
        data.forEach(state => {
          console.log(data);
          if (state.state === 2) {
            this.startRapidPoll(state.id);
          }
        });
      })
      .subscribe();
  }


  //Wijzigen naar websockets!!!
  private startRapidPoll(id: string) {
    if (this.pollingQueue.every(val => val !== id)) {
      this.pollingQueue.push(id);
      let tempSub = Observable
        .interval(1000)
        .switchMap(() => this.taskFacade.pollTaskState(id))
        .map(data => {
          this.tasks$ = this.tasks$.map(tasks => {
            let task = tasks.find(val => val.id === id);
            if (task !== undefined) {
              task.currentProgressPercentage = data.currentProgress;
            }
          });
          if (data.state !== 2) {
            let index = this.pollingQueue.indexOf(id);
            if (index > -1) {
              this.pollingQueue.splice(index, 1);
            }
            tempSub.unsubscribe();
          }
        })
        .subscribe();
    }
  }

  ngOnDestroy() {
    if (this.statePollingSub !== undefined) {
      this.statePollingSub.unsubscribe();
    }
  }
}
