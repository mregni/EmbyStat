import { Subscription } from 'rxjs';

/* tslint:disable:component-class-suffix */
import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { Router } from '@angular/router';

@Component({
  selector: 'es-sync-is-running',
  templateUrl: './sync-is-running.component.html',
  styleUrls: ['./sync-is-running.component.scss']
})
export class SyncIsRunningDialog {
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
