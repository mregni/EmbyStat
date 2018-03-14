import { Component, OnInit } from '@angular/core';
import { SystemFacade } from '../../../system/state/facade.system';

@Component({
  selector: 'app-shutdown-dialog',
  templateUrl: './shutdown.dialog.html',
  styleUrls: ['./shutdown.dialog.scss']
})

export class ShutdownComponent implements OnInit {
  constructor(private systemFacade: SystemFacade) { }

  public onYesClick() {
    this.systemFacade.shutdownServer().subscribe();
  }

  ngOnInit() {

  }
}
