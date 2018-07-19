import { Component, Inject, OnDestroy } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { Subscription } from 'rxjs/Subscription';
import { Router } from '@angular/router';

import { TaskFacade } from '../../../task/state/facade.task';
import { Task } from '../../../task/models/task';

@Component({
  selector: 'app-no-type-found',
  templateUrl: './no-type-found.component.html',
  styleUrls: ['./no-type-found.component.scss']
})
export class NoTypeFoundDialog implements OnDestroy {
  private getTasksSub: Subscription;
  private tasks: Task[];

  constructor(
    public dialogRef: MatDialogRef<string>,
    @Inject(MAT_DIALOG_DATA) public data: string,
    private taskFacade: TaskFacade,
    private router: Router) {
    this.getTasksSub = this.taskFacade.getTasks().subscribe((result: Task[]) => this.tasks = result);
  }

  cancelClick(): void {
    this.dialogRef.close();
  }

  startSyncClick(): void {
    const task = this.tasks.find(x => x.name === 'TASKS.MEDIASYNCTITLE');
    this.taskFacade.fireTask(task.id);
    this.router.navigate(['/task']);
    this.dialogRef.close();

  }

  ngOnDestroy() {
    if (this.getTasksSub !== undefined) {
      this.getTasksSub.unsubscribe();
    }
  }
}
