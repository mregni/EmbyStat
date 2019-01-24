import { Component, OnInit, Output, Input, OnDestroy, EventEmitter } from '@angular/core';
import { MatDialog } from '@angular/material';
import { TriggerDialogComponent } from '../trigger-dialog/trigger-dialog.component';
import { Subscription } from 'rxjs/Subscription';
import * as moment from 'moment';

import { JobSocketService } from '../../shared/services/job-socket.service';
import { JobService } from '../service/job.service';
import { Job } from '../models/job';

@Component({
  selector: 'app-job-item',
  templateUrl: './job-item.component.html',
  styleUrls: ['./job-item.component.scss']
})
export class JobItemComponent implements OnInit, OnDestroy {
  @Output()
  fireAction = new EventEmitter<boolean>();

  @Input()
  id: string;

  private jobSocketSub: Subscription;
  private jobSub: Subscription;

  job: Job;
  startingJob = false;

  constructor(
    public dialog: MatDialog,
    private jobSocketService: JobSocketService,
    private jobService: JobService) {
    this.jobSocketSub = jobSocketService.infoSubject.subscribe((job: Job) => {
      if (job != null && job.id === this.id) {
        this.job = job;
        this.startingJob = false;
      }
    });
  }

  ngOnInit() {
    this.jobSub = this.jobService.getById(this.id).subscribe((job: Job) => {
      if (job != null) {
        this.job = job;
      }
    });
  }

  disableStart() {
    return this.job.state === 1;
  }

  openSettings(): void {
    const dialogRef = this.dialog.open(TriggerDialogComponent, {
      width: '500px',
      data: {
        title: this.job.title,
        description: this.job.description,
        id: this.job.id,
        trigger: this.job.trigger
      }
    });

    dialogRef.afterClosed().subscribe((result: boolean) => {
      if (result) {
        this.jobSub = this.jobService.getById(this.id).subscribe((job: Job) => {
          if (job != null) {
            this.job = job;
          }
        });
      }
    });
  }

  fireJob() {
    this.startingJob = true;
    this.fireAction.emit(true);
  }

  hasHours(time: Date, to = moment.utc()): boolean {
    const from = moment.utc(time);
    to = this.convertToMoment(to);

    let milliseconds = to.diff(from);
    const duration = moment.duration(milliseconds);
    return Math.floor(duration.asHours()) > 0;
  }

  hasMinutes(time: Date, to = moment.utc()): boolean {
    const from = moment.utc(time);
    to = this.convertToMoment(to);

    let milliseconds = to.diff(from);
    const duration = moment.duration(milliseconds);
    return Math.floor(duration.asMinutes()) % 60 > 0;
  }

  hasSeconds(time: Date, to = moment.utc()): boolean {
    const from = moment.utc(time);
    to = this.convertToMoment(to);

    let milliseconds = to.diff(from);
    if (milliseconds < 1000) {
      milliseconds = 1000;
    }
    const duration = moment.duration(milliseconds);
    return (Math.floor(duration.asSeconds()) % 60 + 1) > 0;
  }

  needsAnd(job: Job): boolean {
    return this.hasHours(job.endTimeUtc) && this.hasMinutes(job.endTimeUtc)
  }

  needsAndFor(job: Job): boolean {
    return (this.hasHours(job.startTimeUtc, moment.utc(job.endTimeUtc)) ||
      this.hasMinutes(job.startTimeUtc, moment.utc(job.endTimeUtc))) &&
      this.hasSeconds(job.startTimeUtc, moment.utc(job.endTimeUtc));
  }

  needsCommaFor(job: Job): boolean {
    return this.hasHours(job.startTimeUtc, moment.utc(job.endTimeUtc))
      && this.hasMinutes(job.startTimeUtc, moment.utc(job.endTimeUtc));
  }

  hasNoTime(job: Job): boolean {
    return job.endTimeUtc != null &&
      !this.hasHours(job.endTimeUtc) &&
      !this.hasMinutes(job.endTimeUtc) &&
      !this.hasSeconds(job.endTimeUtc);
  }

  private convertToMoment(value: any) {
    if (value instanceof moment) {
      return value;
    } else {
      return moment.utc(value);
    }
  }

  ngOnDestroy() {
    if (this.jobSocketSub !== undefined) {
      this.jobSocketSub.unsubscribe();
    }

    if (this.jobSub !== undefined) {
      this.jobSub.unsubscribe();
    }
  }
}
