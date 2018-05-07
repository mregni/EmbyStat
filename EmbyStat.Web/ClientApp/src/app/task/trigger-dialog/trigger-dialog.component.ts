import { Component, OnInit, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { Task } from '../models/task';
import { Trigger } from '../models/trigger';
import { UUID } from 'angular2-uuid';

@Component({
  selector: 'app-trigger-dialog',
  templateUrl: './trigger-dialog.component.html',
  styleUrls: ['./trigger-dialog.component.scss']
})
export class TriggerDialogComponent implements OnInit {
  public trigger: Trigger;
  public task: Task;
  public madeChanges = false;

  constructor(public dialogRef: MatDialogRef<TriggerDialogComponent>, @Inject(MAT_DIALOG_DATA) public data: any) {
    this.trigger = new Trigger();
    this.task = data.task;
  }

  public canSafe(): boolean {
    return this.trigger.type !== undefined &&
      (this.trigger.intervalTicks !== undefined || this.trigger.timeOfDayTicks !== undefined);
  }

  public saveTrigger() {
    this.madeChanges = true;
    this.trigger.id = UUID.UUID();
    this.task.triggers.push(this.trigger);
    this.trigger = new Trigger();
  }

  public remove(id: string) {
    this.madeChanges = true;
    const selectedTrigger = this.task.triggers.find(x => x.id === id);
    const index: number = this.task.triggers.indexOf(selectedTrigger);
    if (index !== -1) {
      this.task.triggers.splice(index, 1);
    }
  }

  public saveTriggers() {
    this.dialogRef.close(this.task);
  }

  public cancel() {
    this.dialogRef.close(null);
  }

  ngOnInit() {
  }
}
