import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { HubConnection } from '@aspnet/signalr';

import { ConfigurationFacade } from '../../configuration/state/facade.configuration';
import { Configuration } from '../../configuration/models/configuration';
import { Task } from '../../task/models/task';

@Component({
  selector: 'app-toolbar',
  templateUrl: './toolbar.component.html',
  styleUrls: ['./toolbar.component.scss']
})
export class ToolbarComponent implements OnInit {
  private hubConnection: HubConnection;
  public configuration$: Observable<Configuration>;
  public runningTask: Task;

  @Output() toggleSideNav = new EventEmitter<void>();

  constructor(private configurationFacade: ConfigurationFacade) {
    this.hubConnection = new HubConnection('/tasksignal');
    this.configuration$ = configurationFacade.configuration$;

    this.hubConnection.on('ReceiveInfo', (data: Task[]) => {
      const task = data.find(x => x.state === 2);
      if (data.some(x => x.state === 2)) {
        this.runningTask = task;
      } else {
        this.runningTask = undefined;
      }
    });

    this.hubConnection.on('ReceiveLog', (data: string) => {});
  }

  ngOnInit() {
    this.hubConnection
      .start()
      .then(() => {
        console.log('Connection started!');
      })
      .catch(err => console.log('Error while establishing connection :('));
  }
}
