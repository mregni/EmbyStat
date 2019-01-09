import { Component, OnInit, OnDestroy, Output, EventEmitter } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { Subscription } from 'rxjs/Subscription';
import { ConfigHelper } from '../../helpers/configHelper';

import { ConfigurationFacade } from '../../../configuration/state/facade.configuration';
import { Configuration } from '../../../configuration/models/configuration';
import { Task } from '../../../task/models/task';
import { EmbyStatus } from '../../models/emby/emby-status';
import { TaskSignalService } from '../../services/task-signal.service';
import { EmbyService } from '../../services/emby.service';

@Component({
  selector: 'app-toolbar',
  templateUrl: './toolbar.component.html',
  styleUrls: ['./toolbar.component.scss']
})
export class ToolbarComponent implements OnInit, OnDestroy {
  configuration$: Observable<Configuration>;
  private embyStatusSeb: Subscription;
  private taskInfoSignalSub: Subscription;
  runningTask: Task;

  missedPings: number;

  @Output()
  toggleSideNav = new EventEmitter<void>();

  constructor(
    private configurationFacade: ConfigurationFacade,
    private taskSignalService: TaskSignalService,
    private embyService: EmbyService) {
    this.configuration$ = configurationFacade.configuration$;

    this.missedPings = 0;
    this.taskInfoSignalSub = taskSignalService.infoSubject.subscribe(data => {
      if (data.some(x => x.state === 2)) {
        this.runningTask = data.find(x => x.state === 2);
      } else {
        this.runningTask = undefined;
      }

      if (data.some(x => x.name === 'TASKS.PINGEMBYSERVERTITLE')) {
        this.embyStatusSeb = this.embyService.getEmbyStatus().subscribe((status: EmbyStatus) => {
          this.missedPings = status.missedPings;
        });
        this.embyStatusSeb = undefined;
      }
    });
  }

  getFullAddress(config: Configuration): string {
    return ConfigHelper.getFullEmbyAddress(config);
  }

  ngOnInit() {

  }

  ngOnDestroy() {
    if (this.taskInfoSignalSub !== undefined) {
      this.taskInfoSignalSub.unsubscribe();
    }

    if (this.embyStatusSeb !== undefined) {
      this.embyStatusSeb.unsubscribe();
    }
  }
}
