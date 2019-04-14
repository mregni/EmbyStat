import { Component, NgZone, OnInit, OnDestroy } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import * as signalR from '@aspnet/signalr';

import { SettingsFacade } from './settings/state/facade.settings';
import { JobSocketService } from './shared/services/job-socket.service';
import { UpdateService } from './shared/services/update.service';
import { SideBarService } from './shared/services/side-bar.service';
import { Job } from './jobs/models/job';
import { JobLog } from './jobs/models/job-log';
import { Settings } from './settings/models/settings';

const SMALL_WIDTH_BREAKPOINT = 768;

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit, OnDestroy {
  private mediaMatcher: MediaQueryList = matchMedia(`(max-width: ${SMALL_WIDTH_BREAKPOINT}px)`);
  private settingLoadSub: Subscription;
  settings: Settings;
  openMenu = true;


  constructor(
    private readonly zone: NgZone,
    private readonly settingsFacade: SettingsFacade,
    private readonly translate: TranslateService,
    private readonly router: Router,
    private readonly jobSocketService: JobSocketService,
    private readonly sideBarService: SideBarService,
    private readonly updateService: UpdateService) {
    translate.setDefaultLang('en-US');
    translate.addLangs(['en-US', 'nl-NL', 'de-DE', 'da-DK', 'el-GR', 'es-ES',
      'fi-FI', 'fr-FR', 'hu-HU', 'it-IT', 'no-NO', 'pl-PL', 'pt-BR', 'pt-PT', 'ro-RO',
      'sv-SE', 'cs-CZ']);

    //const hubConnection = new signalR.HubConnectionBuilder()
    //  .withUrl('/jobs-socket')
    //  .build();
    //hubConnection.start().catch(err => document.write(err));

    //hubConnection.on('job-report-progress', (data: Job) => {
    //  jobSocketService.updateJobsInfo(data);
    //});

    //hubConnection.on('job-report-log', (data: JobLog) => {
    //  jobSocketService.updateJobLogs(data.value, data.type);
    //});

    //hubConnection.on('emby-connection-state', (data: number) => {
    //  jobSocketService.updateMissedPings(data);
    //});

    //hubConnection.on('update-state', (state: boolean) => {
    //  if (this.settings !== undefined) {
    //    const copy = { ...this.settings };
    //    copy.updateInProgress = state;
    //    this.settingsFacade.updateSettings(copy);
    //  }
    //});

    sideBarService.menuVisibleSubject.subscribe((state: boolean) => {
      this.openMenu = state;
    });
    this.settings = undefined;
    this.settingLoadSub = this.settingsFacade.getSettings().subscribe((settings: Settings) => {
      if (!settings.wizardFinished) {
        this.router.navigate(['/wizard']);
      }

      this.settings = settings;
      this.translate.use(settings.language);
      this.updateService.setUiToUpdateState(settings.updateInProgress);
    });
  }

  ngOnInit(): void {
    
  }

  ngOnDestroy() {
    if (this.settingLoadSub !== undefined) {
      this.settingLoadSub.unsubscribe();
    }
  }

  isScreenSmall(): boolean {
    return this.mediaMatcher.matches;
  }
}
