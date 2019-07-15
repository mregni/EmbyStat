import { Observable } from 'rxjs';
import { ServerInfo } from 'src/app/shared/models/emby/server-info';

import { Component, OnInit } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';

import { EmbyService } from '../../../shared/services/emby.service';

@Component({
  selector: 'app-server-overview',
  templateUrl: './server-overview.component.html',
  styleUrls: ['./server-overview.component.scss']
})
export class ServerOverviewComponent implements OnInit {
  serverInfo$: Observable<ServerInfo>;

  constructor(private readonly embyService: EmbyService) {
    this.serverInfo$ = this.embyService.getEmbyServerInfo();
  }

  ngOnInit() {
  }
}
