import { Component, Inject, OnDestroy } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { Subscription } from 'rxjs/Subscription';
import { Router } from '@angular/router';

import { TaskSignalService } from '../../services/signalR/task-signal.service';
import { Task } from '../../../task/models/task';

@Component({
  selector: 'app-sync-is-running',
  templateUrl: './sync-is-running.component.html',
  styleUrls: ['./sync-is-running.component.scss']
})
export class SyncIsRunningDialog implements OnDestroy {
  private getTaskInfuSub: Subscription;
  private taskInfoSignalSub: Subscription;
  private tasks: Task[];

  constructor(
    public dialogRef: MatDialogRef<string>,
    @Inject(MAT_DIALOG_DATA) public data: string,
    private taskSignalService: TaskSignalService,
    private router: Router) {
    this.getTaskInfuSub = this.taskSignalService.infoSubject.subscribe();
  }

  cancelClick(): void {
    this.dialogRef.close();
  }

  goToTasks(): void {
    this.router.navigate(['/task']);
    this.dialogRef.close();
  }

  ngOnDestroy() {
    if (this.getTaskInfuSub !== undefined) {
      this.getTaskInfuSub.unsubscribe();
    }
  }
}
