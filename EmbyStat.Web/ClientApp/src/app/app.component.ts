import { Component, NgZone, OnInit, OnDestroy } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs/Subscription';
import * as signalR from '@aspnet/signalr';

import { ConfigurationFacade } from './configuration/state/facade.configuration';
import { JobSocketService } from './shared/services/job-socket.service';
import { SideBarService } from './shared/services/side-bar.service';
import { Job } from './jobs/models/job';
import { JobLog } from './jobs/models/job-log';

const SMALL_WIDTH_BREAKPOINT = 768;

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit, OnDestroy {
  private mediaMatcher: MediaQueryList = matchMedia(`(max-width: ${SMALL_WIDTH_BREAKPOINT}px)`);
  private configChangedSub: Subscription;
  private configLoadSub: Subscription;
  openMenu = true;

  constructor(
    private zone: NgZone,
    private configurationFacade: ConfigurationFacade,
    private translate: TranslateService,
    private router: Router,
    private jobSocketService: JobSocketService,
    private sideBarService: SideBarService) {
    this.mediaMatcher.addListener(mql => zone.run(() => this.mediaMatcher = mql));

    translate.setDefaultLang('en-US');
    translate.addLangs(['en-US', 'nl-NL']);

    const hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('/jobs-socket')
      .build();
    hubConnection.start().catch(err => document.write(err));

    hubConnection.on('job-report-progress', (data: Job) => {
      console.log(data);
      jobSocketService.updateJobsInfo(data);
    });

    hubConnection.on('job-report-log', (data: JobLog) => {
      jobSocketService.updateJobLogs(data.value, data.type);
    });

    hubConnection.on('emby-connection-status', (data: number) => {
      jobSocketService.updateMissedPings(data);
    });

    sideBarService.menuVisibleSubject.subscribe((state: boolean) => {
      this.openMenu = state;
    });
  }

  ngOnInit(): void {
    this.configLoadSub = this.configurationFacade.getConfiguration().subscribe(config => {
      if (!config.wizardFinished) {
        this.router.navigate(['/wizard']);
      }
    });

    this.configChangedSub = this.configurationFacade.configuration$.subscribe(config => {
      this.translate.use(config.language);
      console.log("lang changed");
    });
  }

  ngOnDestroy() {
    if (this.configChangedSub !== undefined) {
      this.configChangedSub.unsubscribe();
    }

    if (this.configLoadSub !== undefined) {
      this.configLoadSub.unsubscribe();
    }
  }

  isScreenSmall(): boolean {
    return this.mediaMatcher.matches;
  }
}
