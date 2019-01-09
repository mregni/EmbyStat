import { Injectable } from '@angular/core';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { BehaviorSubject } from 'rxjs/BehaviorSubject';

import { Task } from '../../task/models/task';

import * as moment from 'moment';

@Injectable()

export class TaskSignalService {
  private lines: SafeHtml[];
  logsSubject = new BehaviorSubject<SafeHtml[]>([]);
  infoSubject = new BehaviorSubject<Task[]>([]);
  isSyncRunning = false;

  constructor(private sanitizer: DomSanitizer) {
    this.lines = [];
  }

  updateTasksLogs(value: string, type: number) {
    const now = moment().format('HH:mm:ss');
    const line = now + ' - ' + value;

    if (type === 1) {
      this.lines.push(this.sanitizer.bypassSecurityTrustHtml('<span class="text__accent">' + line + '</span>'));
    } else if (type === 2) {
      this.lines.push(this.sanitizer.bypassSecurityTrustHtml('<span class="text__warn">' + line + '</span>'));
    } else {
      this.lines.push(line);
    }

    if (this.lines.length >= 20) {
      this.lines.shift();
    }

    this.logsSubject.next(this.lines);
  }

  updateTasksInfo(tasks: Task[]) {
    const task = tasks.find(x => x.name === 'TASKS.MEDIASYNCTITLE');
    this.isSyncRunning = !!task && task.state === 2;

    this.infoSubject.next(tasks);
  }
}
