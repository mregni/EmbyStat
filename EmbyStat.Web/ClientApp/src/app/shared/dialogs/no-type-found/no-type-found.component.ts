import { Subscription } from 'rxjs';

/* tslint:disable:component-class-suffix */
import { Component, Inject, OnDestroy } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { Router } from '@angular/router';

import { JobService } from '../../services/job.service';

@Component({
  selector: 'es-no-type-found',
  templateUrl: './no-type-found.component.html',
  styleUrls: ['./no-type-found.component.scss']
})
export class NoTypeFoundDialog implements OnDestroy {
  private jobSub: Subscription;

  disableButtons = false;

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
    this.disableButtons = true;
    this.jobSub = this.jobService.fireJob('be68900b-ee1d-41ef-b12f-60ef3106052e').subscribe(() => {
      this.router.navigate(['/jobs']);
      this.dialogRef.close();
    });
  }

  ngOnDestroy() {
    if (this.jobSub !== undefined) {
      this.jobSub.unsubscribe();
    }
  }
}
