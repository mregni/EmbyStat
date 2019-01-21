/* tslint:disable:component-class-suffix */
import { Component, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { Subscription } from 'rxjs/Subscription';
import { Router } from '@angular/router';

@Component({
  selector: 'app-sync-is-running',
  templateUrl: './sync-is-running.component.html',
  styleUrls: ['./sync-is-running.component.scss']
})
export class SyncIsRunningDialog {
  private getTaskInfuSub: Subscription;

  constructor(
    public dialogRef: MatDialogRef<string>,
    @Inject(MAT_DIALOG_DATA) public data: string,
    private router: Router) {
  }

  cancelClick(): void {
    this.dialogRef.close();
  }

  goToJobs(): void {
    this.router.navigate(['/jobs']);
    this.dialogRef.close();
  }
}
