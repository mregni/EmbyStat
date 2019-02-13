import { Component, OnInit, OnDestroy } from '@angular/core';
import { SafeHtml } from '@angular/platform-browser';
import { Subscription } from 'rxjs';

import { JobSocketService } from '../../shared/services/job-socket.service';
import { JobService } from '../service/job.service';
import { Job } from '../models/job';

@Component({
  selector: 'app-jobs-overview',
  templateUrl: './jobs-overview.component.html',
  styleUrls: ['./jobs-overview.component.scss']
})
export class JobsOverviewComponent implements OnInit, OnDestroy {
  private getTasksSub: Subscription;
  private jobLogsSignalSub: Subscription;
  private fireJobSub: Subscription;

  lines: SafeHtml[] = [];
  jobs: Job[];

  constructor(
    private jobService: JobService,
    private jobSocketService: JobSocketService) {

    this.jobLogsSignalSub = jobSocketService.logsSubject.subscribe(logs => {
      this.lines = logs;
    });

  }

  ngOnInit() {
    
  }

  firePing() {
    this.fireJobSub = this.jobService.firePingJob().subscribe();
  }

  fireCheckUpdate() {
    this.fireJobSub = this.jobService.fireCheckUpdateJob().subscribe();
  }

  fireMediaSync() {
    this.fireJobSub = this.jobService.fireMediaSyncJob().subscribe();
  }

  fireSmallSync() {
    this.fireJobSub = this.jobService.fireSmallSyncJob().subscribe();
  }
  
  fireDatabaseCleanup() {
    this.fireJobSub = this.jobService.fireDatabaseCleanupJob().subscribe();
  }

  ngOnDestroy() {
    if (this.getTasksSub !== undefined) {
      this.getTasksSub.unsubscribe();
    }

    if (this.jobLogsSignalSub !== undefined) {
      this.jobLogsSignalSub.unsubscribe();
    }

    if (this.fireJobSub !== undefined) {
      this.fireJobSub.unsubscribe();
    }
  }
}
