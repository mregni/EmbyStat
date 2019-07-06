import { Subscription } from 'rxjs';

import { Component, OnDestroy, OnInit } from '@angular/core';
import { MatDialogRef } from '@angular/material';
import { Router } from '@angular/router';

import { JobService } from '../../services/job.service';

@Component({
// tslint:disable-next-line: component-selector
  selector: 'no-users-found-dialog',
  templateUrl: './no-users-found-dialog.html',
  styleUrls: ['./no-users-found-dialog.scss']
})
// tslint:disable-next-line: component-class-suffix
export class NoUsersFoundDialog implements OnInit, OnDestroy {
  private jobSub: Subscription;

  disableButtons = false;

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
    this.disableButtons = true;
    this.jobSub = this.jobService.fireJob('41e0bf22-1e6b-4f5d-90be-ec966f746a2f').subscribe(() => {
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
