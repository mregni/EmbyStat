/* tslint:disable:component-class-suffix */
import { Component, Inject, OnDestroy } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { Subscription } from 'rxjs/Subscription';
import { Router } from '@angular/router';

import { JobSocketService } from '../../services/job-socket.service';

@Component({
  selector: 'app-sync-is-running',
  templateUrl: './sync-is-running.component.html',
  styleUrls: ['./sync-is-running.component.scss']
})
export class SyncIsRunningDialog implements OnDestroy {
  private getTaskInfuSub: Subscription;

  constructor(
    public dialogRef: MatDialogRef<string>,
    @Inject(MAT_DIALOG_DATA) public data: string,
    private jobSocketService: JobSocketService,
    private router: Router) {
    this.getTaskInfuSub = this.jobSocketService.infoSubject.subscribe();
  }

  cancelClick(): void {
    this.dialogRef.close();
  }

  goToJobs(): void {
    this.router.navigate(['/job']);
    this.dialogRef.close();
  }

  ngOnDestroy() {
    if (this.getTaskInfuSub !== undefined) {
      this.getTaskInfuSub.unsubscribe();
    }
  }
}
