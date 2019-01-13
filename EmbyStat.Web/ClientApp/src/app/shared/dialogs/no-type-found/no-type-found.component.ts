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
  private getTasksSub: Subscription;

  constructor(
    public dialogRef: MatDialogRef<string>,
    @Inject(MAT_DIALOG_DATA) public data: string,
    private jobService: JobService,
    private router: Router) {
    //this.getTasksSub = this.taskService.getTasks().subscribe((result: Task[]) => this.tasks = result);
  }

  cancelClick(): void {
    this.dialogRef.close();
  }

  startSyncClick(): void {
    //this.taskService.fireTask(task.id);
    this.router.navigate(['/task']);
    this.dialogRef.close();

  }

  ngOnDestroy() {
    if (this.getTasksSub !== undefined) {
      this.getTasksSub.unsubscribe();
    }
  }
}
