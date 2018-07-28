import { Component, OnInit, OnDestroy } from '@angular/core';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { TaskFacade } from './state/facade.task';
import { Subscription } from 'rxjs/Subscription';
import { Task } from './models/task';
import { MatDialog } from '@angular/material';
import { TriggerDialogComponent } from './trigger-dialog/trigger-dialog.component';

import 'rxjs/Rx';
import * as moment from 'moment';
import * as signalR from "@aspnet/signalr";

import { ProgressLog } from './models/progressLog';

@Component({
  selector: 'app-task',
  templateUrl: './task.component.html',
  styleUrls: ['./task.component.scss']
})
export class TaskComponent implements OnInit, OnDestroy {
  private getTasksSub: Subscription;
  public tasks: Task[];
  public lines: SafeHtml[] = [];

  constructor(private taskFacade: TaskFacade,
    public dialog: MatDialog,
    private sanitizer: DomSanitizer) {
    const hubConnection = new signalR.HubConnectionBuilder()
      .withUrl("/tasksignal")
      .build();
    hubConnection.start().catch(err => document.write(err));

    hubConnection.on('ReceiveInfo', (data: Task[]) => {
      this.tasks = data;
    });

    hubConnection.on('ReceiveLog', (data: ProgressLog) => {
      console.log("BOE");
      const now = moment().format('HH:mm:ss');
      var line = now + ' - ' + data.value;

      if (data.type === 1) {
        this.lines.push(sanitizer.bypassSecurityTrustHtml('<span class="text__accent">' + line + '</span>'));
      } else if (data.type === 2) {
        this.lines.push(sanitizer.bypassSecurityTrustHtml('<span class="text__warn">' + line + '</span>'));
      } else {
        this.lines.push(line);
      }

      if (this.lines.length >= 15) {
        this.lines.shift();
      }
    });
  }

  ngOnInit() {
    this.getTasksSub = this.taskFacade.getTasks().subscribe(data => this.tasks = data);
  }

  public openDialog(task: Task): void {
    const dialogRef = this.dialog.open(TriggerDialogComponent, {
      width: '500px',
      data: { task: task }
    });

    dialogRef.afterClosed().subscribe((result: Task) => {
      if (result !== null) {
        this.taskFacade.updateTrigger(result).subscribe(data => {
          this.tasks = data;
        });
      } else {
        this.getTasksSub = this.taskFacade.getTasks().subscribe(data => this.tasks = data);
      }
    });
  }

  public hasHours(time: Date, to = moment.utc()): boolean {
    const from = moment.utc(time);
    to = this.convertToMoment(to);

    const milliseconds = to.diff(from);
    const duration = moment.duration(milliseconds);
    return Math.floor(duration.asHours()) > 0;
  }

  public hasMinutes(time: Date, to = moment.utc()): boolean {
    const from = moment.utc(time);
    to = this.convertToMoment(to);

    const milliseconds = to.diff(from);
    const duration = moment.duration(milliseconds);
    return Math.floor(duration.asMinutes()) % 60 > 0;
  }

  public hasSeconds(time: Date, to = moment.utc()): boolean {
    const from = moment.utc(time);
    to = this.convertToMoment(to);

    const milliseconds = to.diff(from);
    const duration = moment.duration(milliseconds);
    return (Math.floor(duration.asSeconds()) % 60 + 1) > 0;
  }

  public needsAnd(task: Task): boolean {
    return (this.hasHours(task.lastExecutionResult.endTimeUtc) ||
      this.hasMinutes(task.lastExecutionResult.endTimeUtc)) &&
      this.hasSeconds(task.lastExecutionResult.endTimeUtc);
  }

  public needsAndFor(task: Task): boolean {
    return (this.hasHours(task.lastExecutionResult.startTimeUtc, moment.utc(task.lastExecutionResult.endTimeUtc)) ||
      this.hasMinutes(task.lastExecutionResult.startTimeUtc, moment.utc(task.lastExecutionResult.endTimeUtc))) &&
      this.hasSeconds(task.lastExecutionResult.startTimeUtc, moment.utc(task.lastExecutionResult.endTimeUtc));
  }

  public needsComma(task: Task): boolean {
    return this.hasHours(task.lastExecutionResult.endTimeUtc) && this.hasMinutes(task.lastExecutionResult.endTimeUtc);
  }

  public needsCommaFor(task: Task): boolean {
    return this.hasHours(task.lastExecutionResult.startTimeUtc, moment.utc(task.lastExecutionResult.endTimeUtc))
      && this.hasMinutes(task.lastExecutionResult.startTimeUtc, moment.utc(task.lastExecutionResult.endTimeUtc));
  }

  public hasNoTime(task: Task): boolean {
    return !this.hasHours(task.lastExecutionResult.endTimeUtc) &&
           !this.hasMinutes(task.lastExecutionResult.endTimeUtc) &&
           !this.hasSeconds(task.lastExecutionResult.endTimeUtc);
  }

  public fireTask(id: string): void {
    this.taskFacade.fireTask(id);
  }

  private convertToMoment(value: any) {
    if (value instanceof moment) {
      return value;
    } else {
      return moment.utc(value);
    }
  }

  ngOnDestroy() {
    if (this.getTasksSub !== undefined) {
      this.getTasksSub.unsubscribe();
    }
  }
}
