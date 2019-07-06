import { Observable, Subscription } from 'rxjs';

import { Component, EventEmitter, OnDestroy, OnInit, Output } from '@angular/core';

import { SettingsFacade } from '../../facades/settings.facade';
import { ConfigHelper } from '../../helpers/config-helper';
import { Job } from '../../models/jobs/job';
import { Settings } from '../../models/settings/settings';
import { EmbyService } from '../../services/emby.service';
import { JobSocketService } from '../../services/job-socket.service';

@Component({
  selector: 'app-toolbar',
  templateUrl: './toolbar.component.html',
  styleUrls: ['./toolbar.component.scss']
})
export class ToolbarComponent implements OnInit, OnDestroy {
  settings$: Observable<Settings>;
  private embyStatusSeb: Subscription;
  private jobSocketSub: Subscription;
  private missedPingsSub: Subscription;
  runningJob: Job;

  missedPings: number;

  @Output()
  toggleSideNav = new EventEmitter<void>();

  constructor(
    private settingsFacade: SettingsFacade,
    private jobSocketService: JobSocketService,
    private embyService: EmbyService) {
    this.settings$ = settingsFacade.getSettings();

    this.missedPings = 0;
    this.jobSocketSub = jobSocketService.infoSubject.subscribe((job: Job) => {
      if (job != null && job.state === 1) {
        this.runningJob = job;
      } else {
        this.runningJob = undefined;
      }
    });

    this.missedPingsSub = jobSocketService.missedPingsSubject.subscribe((count: number) => {
      this.missedPings = count;
    });
  }

  getFullAddress(settings: Settings): string {
    return ConfigHelper.getFullEmbyAddress(settings);
  }

  ngOnInit() {

  }

  ngOnDestroy() {
    if (this.jobSocketSub !== undefined) {
      this.jobSocketSub.unsubscribe();
    }

    if (this.embyStatusSeb !== undefined) {
      this.embyStatusSeb.unsubscribe();
    }

    if (this.missedPingsSub !== undefined) {
      this.missedPingsSub.unsubscribe();
    }
  }
}
