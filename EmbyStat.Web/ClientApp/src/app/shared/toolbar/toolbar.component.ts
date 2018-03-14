import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { MatDialog } from '@angular/material';
import { ShutdownComponent } from '../dialog/shutdown/shutdown.dialog';

@Component({
  selector: 'app-toolbar',
  templateUrl: './toolbar.component.html',
  styleUrls: ['./toolbar.component.scss']
})
export class ToolbarComponent implements OnInit {

  @Output() toggleSideNav = new EventEmitter<void>();
  constructor(public dialog: MatDialog) { }

  public openShutdownDialog() {
    this.dialog.open(ShutdownComponent);
  }

  ngOnInit() {

  }
}
