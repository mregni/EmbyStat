/* tslint:disable:component-class-suffix */
import { Component, Inject, OnDestroy } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { Subscription } from 'rxjs/Subscription';
import { Router } from '@angular/router';

import { JobService } from '../../../jobs/service/job.service';

@Component({
  selector: 'app-no-type-found',
  templateUrl: './no-type-found.component.html',
  styleUrls: ['./no-type-found.component.scss']
})
export class NoTypeFoundDialog implements OnDestroy {
  private jobSub: Subscription;

  constructor(
    public dialogRef: MatDialogRef<string>,
    @Inject(MAT_DIALOG_DATA) public data: string,
    private jobService: JobService,
    private router: Router) {
  }

  cancelClick(): void {
    this.dialogRef.close();
  }

  startMediaSync(): void {
    this.jobSub = this.jobService.fireMediaSyncJob().subscribe();
    this.router.navigate(['/jobs']);
    this.dialogRef.close();

  }

  ngOnDestroy() {
    if (this.jobSub !== undefined) {
      this.jobSub.unsubscribe();
    }
  }
}
