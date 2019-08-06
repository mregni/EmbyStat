import { Observable } from 'rxjs';
import { ServerInfo } from 'src/app/shared/models/emby/server-info';

import { Component, OnInit } from '@angular/core';

import { EmbyServerInfoFacade } from '../../../shared/facades/emby-server.facade';

@Component({
  selector: 'app-server-overview',
  templateUrl: './server-overview.component.html',
  styleUrls: ['./server-overview.component.scss']
})
export class ServerOverviewComponent implements OnInit {
  serverInfo$: Observable<ServerInfo>;

  constructor(private readonly embyServerInfoFacade: EmbyServerInfoFacade) {
    this.serverInfo$ = this.embyServerInfoFacade.getEmbyServerInfo();
  }

  ngOnInit() {
  }
}
