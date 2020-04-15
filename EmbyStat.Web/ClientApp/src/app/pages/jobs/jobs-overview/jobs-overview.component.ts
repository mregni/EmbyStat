import { Observable, Subscription } from 'rxjs';
import { Job } from 'src/app/shared/models/jobs/job';

import { Component, OnDestroy } from '@angular/core';
import { SafeHtml } from '@angular/platform-browser';

import { JobSocketService } from '../../../shared/services/job-socket.service';
import { JobService } from '../../../shared/services/job.service';

@Component({
  selector: 'es-jobs-overview',
  templateUrl: './jobs-overview.component.html',
  styleUrls: ['./jobs-overview.component.scss']
})
export class JobsOverviewComponent implements OnDestroy {
  jobs$: Observable<Job[]>;
  private jobLogsSignalSub: Subscription;

  lines: SafeHtml[] = [];

  constructor(
    private readonly jobService: JobService,
    private readonly jobSocketService: JobSocketService) {
    this.jobs$ = this.jobService.getAll();

    this.jobLogsSignalSub = this.jobSocketService.logsSubject.subscribe(logs => {
      this.lines = logs;
    });
  }

  ngOnDestroy(): void {
    if (this.jobLogsSignalSub !== undefined) {
      this.jobLogsSignalSub.unsubscribe();
    }
  }
}
