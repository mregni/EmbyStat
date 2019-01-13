import { Component, OnInit, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';

@Component({
  selector: 'app-trigger-dialog',
  templateUrl: './trigger-dialog.component.html',
  styleUrls: ['./trigger-dialog.component.scss']
})
export class TriggerDialogComponent implements OnInit {
  madeChanges = false;
  title: string;
  description: string;

  constructor(
    private dialogRef: MatDialogRef<TriggerDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    this.title = data.title;
    this.description = data.description;
  }

  saveSettings() {
    this.dialogRef.close();
  }

  cancel() {
    this.dialogRef.close(null);
  }

  ngOnInit() {
  }
}
