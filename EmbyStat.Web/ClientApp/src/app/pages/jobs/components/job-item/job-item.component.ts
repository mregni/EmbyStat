import * as moment from 'moment';
import { Subscription } from 'rxjs';

import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';

import { Job } from '../../../../shared/models/jobs/job';
import { JobSocketService } from '../../../../shared/services/job-socket.service';
import { JobService } from '../../../../shared/services/job.service';
import { ToastService } from '../../../../shared/services/toast.service';
import { TriggerDialogComponent } from '../trigger-dialog/trigger-dialog.component';

@Component({
  selector: 'app-job-item',
  templateUrl: './job-item.component.html',
  styleUrls: ['./job-item.component.scss']
})
export class JobItemComponent implements OnInit, OnDestroy {
  @Input()
  set job(value: Job) {
    this._job = value;
  }

  private jobSocketSub: Subscription;
  private jobSub: Subscription;

  startingJob = false;
  _job: Job;

  constructor(
    public readonly dialog: MatDialog,
    private readonly jobSocketService: JobSocketService,
    private readonly jobService: JobService,
    private readonly toastService: ToastService
  ) {
    this.jobSocketSub = this.jobSocketService.infoSubject.subscribe((job: Job) => {
        if (job != null && this._job !== undefined && job.id === this._job.id) {
        this._job = job;
        this.startingJob = false;
      }
    });
  }

  ngOnInit() {
  }

  disableStart() {
    return this._job.state === 1;
  }

  openSettings(): void {
    const dialogRef = this.dialog.open(TriggerDialogComponent, {
      width: '500px',
      data: {
        title: this._job.title,
        description: this._job.description,
        id: this._job.id,
        trigger: this._job.trigger
      }
    });

    dialogRef.afterClosed().subscribe((result: boolean) => {
      if (result) {
        this.toastService.showSuccess('JOB.CRONUPDATED');
        this.jobSub = this.jobService.getById(this._job.id).subscribe((job: Job) => {
          if (job != null) {
            this._job = job;
          }
        });
      }
    });
  }

  fireJob() {
    this.startingJob = true;
    this.jobSub = this.jobService.fireJob(this._job.id).subscribe();
  }

  hasHours(time: moment.Moment, to = moment.utc()): boolean {
    const from = moment.utc(time);
    to = this.convertToMoment(to);

    const milliseconds = to.diff(from);
    const durationDiff = moment.duration(milliseconds);
    return Math.floor(durationDiff.asHours()) > 0;
  }

  hasMinutes(time: moment.Moment, to = moment.utc()): boolean {
    const from = moment.utc(time);
    to = this.convertToMoment(to);

    const milliseconds = to.diff(from);
    const durationDiff = moment.duration(milliseconds);
    return Math.floor(durationDiff.asMinutes()) % 60 > 0;
  }

  hasSeconds(time: moment.Moment, to = moment.utc()): boolean {
    const from = moment.utc(time);
    to = this.convertToMoment(to);

    let milliseconds = to.diff(from);
    if (milliseconds < 1000) {
      milliseconds = 1000;
    }
    const durationDiff = moment.duration(milliseconds);
    return (Math.floor(durationDiff.asSeconds()) % 60 + 1) > 0;
  }

  needsAnd(): boolean {
    return this.hasHours(this._job.endTimeUtc) && this.hasMinutes(this._job.endTimeUtc);
  }

  needsAndFor(): boolean {
    return (this.hasHours(this._job.startTimeUtc, moment.utc(this._job.endTimeUtc)) ||
      this.hasMinutes(this._job.startTimeUtc, moment.utc(this._job.endTimeUtc))) &&
      this.hasSeconds(this._job.startTimeUtc, moment.utc(this._job.endTimeUtc));
  }

  needsCommaFor(): boolean {
    return this.hasHours(this._job.startTimeUtc, moment.utc(this._job.endTimeUtc))
      && this.hasMinutes(this._job.startTimeUtc, moment.utc(this._job.endTimeUtc));
  }

  hasNoTime(): boolean {
    return this._job.endTimeUtc != null &&
      !this.hasHours(this._job.endTimeUtc) &&
      !this.hasMinutes(this._job.endTimeUtc) &&
      !this.hasSeconds(this._job.endTimeUtc);
  }

  private convertToMoment(value: any): moment.Moment {
    if (value instanceof moment) {
      return moment(value);
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
