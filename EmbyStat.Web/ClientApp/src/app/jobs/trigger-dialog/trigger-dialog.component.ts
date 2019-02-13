import { Component, OnInit, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { Subscription } from 'rxjs';

import { JobService } from '../service/job.service';

@Component({
  selector: 'app-trigger-dialog',
  templateUrl: './trigger-dialog.component.html',
  styleUrls: ['./trigger-dialog.component.scss']
})
export class TriggerDialogComponent implements OnInit {
  private updateTriggerSub: Subscription;
  private cronPattern = "^(([*])|((([*]|([0-9]|[1-5][0-9]))[/])*([0-9]|[1-5][0-9]))|(([0-9]|[1-5][1-9])([,]([0-9]|[1-5][0-9]))+)|(([0-9]|[1-5][0-9])-([0-9]|[1-5][0-9]))) (([*])|([0-9]|1[0-9]|2[0-3])|((([*]|([0-9]|1[0-9]|2[0-3]))[/])*([0-9]|1[0-9]|2[0-3]))|((([0-9]|1[0-9]|2[0-3])([,]([0-9]|1[0-9]|2[0-3]))+))|(([0-9]|1[0-9]|2[0-3])-([0-9]|1[0-9]|2[0-3]))) (([*])|([1-9]|[1-2][0-9]|3[0-1])|((([*]|([1-9]|[1-2][0-9]|3[0-1]))[/])*([1-9]|[1-2][0-9]|3[0-1]))|((([1-9]|[1-2][0-9]|3[0-1])([,]([1-9]|[1-2][0-9]|3[0-1]))+))|(([1-9]|[1-2][0-9]|3[0-1])-([1-9]|[1-2][0-9]|3[0-1]))) (([*])|([1-9]|1[0-2])|((([*]|([1-9]|1[0-2]))[/])*([1-9]|1[0-2]))|((([1-9]|1[0-2])([,]([1-9]|1[0-2]))+))|(([1-9]|1[0-2])-([1-9]|1[0-2]))) (([*])|([0-7])|([0-7]([,][0-7])+)|([0-7]-[0-7]))$";
  disableButtons = false;

  madeChanges = false;
  title: string;
  description: string;
  id: string;

  form: FormGroup;
  cronControl = new FormControl('', [Validators.required, Validators.pattern(this.cronPattern)]);

  constructor(
    private dialogRef: MatDialogRef<TriggerDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,
    private jobService: JobService) {
    this.title = data.title;
    this.description = data.description;
    this.id = data.id;
    this.cronControl.setValue(data.trigger);
  }

  ngOnInit() {
    this.form = new FormGroup({
      cron: this.cronControl
    });
  }

  saveCron() {
    for (let i in this.form.controls) {
      this.form.controls[i].markAsTouched();
      this.form.controls[i].updateValueAndValidity();
    }

    if (this.form.valid) {
      this.disableButtons = true;
      this.updateTriggerSub = this.jobService.updateTrigger(this.id, this.cronControl.value).subscribe(() => {
        this.disableButtons = false;
        this.dialogRef.close();
      });
    }
  }

  cancel() {
    this.dialogRef.close(null);
  }
}
