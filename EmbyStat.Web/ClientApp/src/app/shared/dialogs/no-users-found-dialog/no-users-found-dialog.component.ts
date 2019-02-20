import { Component, OnInit, OnDestroy } from '@angular/core';
import { MatDialogRef } from '@angular/material';
import { Subscription } from 'rxjs';
import { Router } from '@angular/router';

import { JobService } from '../../../jobs/service/job.service';

@Component({
  selector: 'no-users-found-dialog',
  templateUrl: './no-users-found-dialog.component.html',
  styleUrls: ['./no-users-found-dialog.component.scss']
})
export class NoUsersFoundDialogComponent implements OnInit, OnDestroy {
  private jobSub: Subscription;

  constructor(
    private readonly dialogRef: MatDialogRef<string>,
    private readonly router: Router,
    private readonly jobService: JobService) { }

  ngOnInit() {
  }

  cancelClick(): void {
    this.dialogRef.close();
  }

  startSmallSync(): void {
    this.jobSub = this.jobService.fireSmallSyncJob().subscribe();
    this.router.navigate(['/jobs']);
    this.dialogRef.close();
  }

  ngOnDestroy() {
    if (this.jobSub !== undefined) {
      this.jobSub.unsubscribe();
    }
  }
}
