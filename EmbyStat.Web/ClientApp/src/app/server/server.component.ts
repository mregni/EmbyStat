import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs/Observable';

import { ServerFacade } from './state/facade.server';
import { ServerInfo } from './models/serverInfo';
@Component({
  selector: 'app-server',
  templateUrl: './server.component.html',
  styleUrls: ['./server.component.scss']
})
export class ServerComponent implements OnInit {
  public serverInfo$: Observable<ServerInfo>;

  constructor(private serverFacade: ServerFacade) {
    this.serverInfo$ = this.serverFacade.getServerInfo();
  }

  ngOnInit() {
  }

}
