import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';

import { ServerFacade } from '../state/facade.server';
import { ServerInfo } from '../../shared/models/emby/server-info';

@Component({
  selector: 'app-server-overview',
  templateUrl: './server-overview.component.html',
  styleUrls: ['./server-overview.component.scss']
})
export class ServerOverviewComponent implements OnInit {
  serverInfo$: Observable<ServerInfo>;

  constructor(private serverFacade: ServerFacade) {
    this.serverInfo$ = this.serverFacade.getServerInfo();
  }

  ngOnInit() {
  }

}
