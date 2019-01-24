import { Component, NgZone, OnInit, OnDestroy } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs/Subscription';
import * as signalR from '@aspnet/signalr';

import { ConfigurationFacade } from './configuration/state/facade.configuration';
import { JobSocketService } from './shared/services/job-socket.service';
import { UpdateService } from './shared/services/update.service';
import { SideBarService } from './shared/services/side-bar.service';
import { Job } from './jobs/models/job';
import { JobLog } from './jobs/models/job-log';
import { Configuration } from './configuration/models/configuration';

const SMALL_WIDTH_BREAKPOINT = 768;

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit, OnDestroy {
  private mediaMatcher: MediaQueryList = matchMedia(`(max-width: ${SMALL_WIDTH_BREAKPOINT}px)`);
  private configLoadSub: Subscription;
  configuration: Configuration;
  openMenu = true;


  constructor(
    private zone: NgZone,
    private configurationFacade: ConfigurationFacade,
    private translate: TranslateService,
    private router: Router,
    private jobSocketService: JobSocketService,
    private sideBarService: SideBarService,
    private updateService: UpdateService) {
    this.mediaMatcher.addListener(mql => zone.run(() => this.mediaMatcher = mql));

    translate.setDefaultLang('en-US');
    translate.addLangs(['en-US', 'nl-NL', 'de-DE', 'da-DK', 'el-GR', 'es-ES',
      'fi-FI', 'fr-FR', 'hu-HU', 'it-IT', 'no-NO', 'pl-PL', 'pt-BR', 'pt-PT', 'ro-RO', 'sv-SE']);

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

    hubConnection.on('emby-connection-state', (data: number) => {
      jobSocketService.updateMissedPings(data);
    });

    hubConnection.on('update-state', (state: boolean) => {
      console.log("update-state triggered");
      if (this.configuration !== undefined) {
        console.log("config updated");
        const copy = { ...this.configuration };
        copy.updateInProgress = state;
        this.configurationFacade.updateConfiguration(copy);
      }
    });

    sideBarService.menuVisibleSubject.subscribe((state: boolean) => {
      this.openMenu = state;
    });
  }

  ngOnInit(): void {
    this.configLoadSub = this.configurationFacade.getConfiguration().subscribe(config => {
      this.configuration = config;
      this.translate.use(config.language);

      if (!config.wizardFinished) {
        this.router.navigate(['/wizard']);
      }

      this.updateService.setUiToUpdateState(config.updateInProgress);
    });
  }

  ngOnDestroy() {
    if (this.configLoadSub !== undefined) {
      this.configLoadSub.unsubscribe();
    }
  }

  isScreenSmall(): boolean {
    return this.mediaMatcher.matches;
  }
}
