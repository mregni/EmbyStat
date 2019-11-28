import * as moment from 'moment';
import { BehaviorSubject } from 'rxjs';

import { Injectable } from '@angular/core';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';

import { Job } from '../models/jobs/job';

@Injectable()
export class JobSocketService {
  private lines: SafeHtml[];
  logsSubject = new BehaviorSubject<SafeHtml[]>([]);
  infoSubject = new BehaviorSubject<Job>(null);
  missedPingsSubject = new BehaviorSubject<number>(0);

  constructor(private sanitizer: DomSanitizer) {
    this.lines = [];
  }

  updateJobLogs(value: string, type: number) {
    const now = moment().format('HH:mm:ss DD-MM-YY');
      const line = `${now} ${value}`;

    if (type === 1) {
      this.lines.push(this.sanitizer.bypassSecurityTrustHtml(`<span class="text-accent">${line}</span>`));
    } else if (type === 2) {
      this.lines.push(this.sanitizer.bypassSecurityTrustHtml(`<span class="text-warn">${line}</span>`));
    } else {
      this.lines.push(line);
    }

    if (this.lines.length >= 20) {
      this.lines.shift();
    }

    this.logsSubject.next(this.lines);
  }

  updateJobsInfo(job: Job) {
    this.infoSubject.next(job);
  }

  updateMissedPings(count: number) {
    this.missedPingsSubject.next(count);
  }
}
