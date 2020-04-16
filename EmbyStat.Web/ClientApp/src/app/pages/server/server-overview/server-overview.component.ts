import { Observable } from 'rxjs';

import { Component } from '@angular/core';

import { EmbyServerInfoFacade } from '../../../shared/facades/emby-server.facade';
import { ServerInfo } from '../../../shared/models/media-server/server-info';

@Component({
  selector: 'es-server-overview',
  templateUrl: './server-overview.component.html',
  styleUrls: ['./server-overview.component.scss']
})
export class ServerOverviewComponent {
  serverInfo$: Observable<ServerInfo>;

  constructor(private readonly embyServerInfoFacade: EmbyServerInfoFacade) {
    this.serverInfo$ = this.embyServerInfoFacade.getEmbyServerInfo();
  }
}
