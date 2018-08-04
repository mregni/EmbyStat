import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { Subscription } from 'rxjs/Subscription';
import * as signalR from "@aspnet/signalr";
import { ConfigHelper } from '../helpers/configHelper';

import { ConfigurationFacade } from '../../configuration/state/facade.configuration';
import { Configuration } from '../../configuration/models/configuration';
import { Task } from '../../task/models/task';
import { EmbyStatus } from '../models/emby/embyStatus';
import { ToolbarFacade } from './state/facade.toolbar';

@Component({
  selector: 'app-toolbar',
  templateUrl: './toolbar.component.html',
  styleUrls: ['./toolbar.component.scss']
})
export class ToolbarComponent implements OnInit {
  public configuration$: Observable<Configuration>;
  private embyStatusSeb: Subscription;
  public runningTask: Task;

  public missedPings: number;

  @Output()
  toggleSideNav = new EventEmitter<void>();

  constructor(private configurationFacade: ConfigurationFacade, private toolbarFacade: ToolbarFacade) {
    this.configuration$ = configurationFacade.configuration$;

    this.missedPings = 0;
    const hubConnection = new signalR.HubConnectionBuilder()
      .withUrl("/tasksignal")
      .build();
    hubConnection.start().catch(err => document.write(err));

    hubConnection.on('ReceiveInfo',
      (data: Task[]) => {
        if (data.some(x => x.state === 2)) {
          this.runningTask = data.find(x => x.state === 2);
        } else {
          this.runningTask = undefined;
        }

        if (data.some(x => x.name === 'TASKS.PINGEMBYSERVERTITLE')) {
          this.embyStatusSeb = this.toolbarFacade.getEmbyStatus().subscribe((status: EmbyStatus) => {
            this.missedPings = status.missedPings;
          });
          this.embyStatusSeb = undefined;
        }
      });

    hubConnection.on('ReceiveLog',
      (data: string) => {

      });
  }

  public getFullAddress(config: Configuration): string {
    return ConfigHelper.getFullEmbyAddress(config);
  }

  ngOnInit() {

  }
}
