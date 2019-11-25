import { Subscription } from 'rxjs';

import { registerLocaleData } from '@angular/common';
import localeCs from '@angular/common/locales/cs';
import localeDa from '@angular/common/locales/da';
import localeDe from '@angular/common/locales/de';
import localeEl from '@angular/common/locales/el';
import localeEn from '@angular/common/locales/en';
import localeEs from '@angular/common/locales/es';
import localeFi from '@angular/common/locales/fi';
import localeFr from '@angular/common/locales/fr';
import localeHu from '@angular/common/locales/hu';
import localeIt from '@angular/common/locales/it';
import localeNo from '@angular/common/locales/ne';
import localeNl from '@angular/common/locales/nl';
import localePl from '@angular/common/locales/pl';
import localePtBr from '@angular/common/locales/pt';
import localePtPt from '@angular/common/locales/pt-PT';
import localeRo from '@angular/common/locales/ro';
import localeSv from '@angular/common/locales/sv';
import { Component, LOCALE_ID, NgZone, OnDestroy, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import * as signalR from '@aspnet/signalr';
import { TranslateService } from '@ngx-translate/core';

import { SettingsFacade } from './shared/facades/settings.facade';
import { Job } from './shared/models/jobs/job';
import { JobLog } from './shared/models/jobs/job-log';
import { Settings } from './shared/models/settings/settings';
import { JobSocketService } from './shared/services/job-socket.service';
import { SideBarService } from './shared/services/side-bar.service';
import { UpdateService } from './shared/services/update.service';

const SMALL_WIDTH_BREAKPOINT = 960;

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

    const hubConnection = new signalR.HubConnectionBuilder()
     .withUrl('/jobs-socket')
     .build();
    hubConnection.start().catch(err => document.write(err));

    hubConnection.on('job-report-progress', (data: Job) => {
     jobSocketService.updateJobsInfo(data);
    });

    hubConnection.on('job-report-log', (data: JobLog) => {
    jobSocketService.updateJobLogs(data.value, data.type);
    });

    hubConnection.on('emby-connection-state', (data: number) => {
     jobSocketService.updateMissedPings(data);
    });

    hubConnection.on('update-state', (state: boolean) => {
     if (this.settings !== undefined) {
       this.settings.updateInProgress = state;
       this.settingsFacade.updateSettings(this.settings);
     }
    });

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
      registerLocaleData(this.getLanguageLocal(settings.language), settings.language);
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

  private getLanguageLocal(language: string) {
    switch (language) {
      case 'en-US': return localeEn;
      case 'nl-NL': return localeNl;
      case 'fr-FR': return localeFr;
      case 'de-DE': return localeDe;
      case 'da-DK': return localeDa;
      case 'el-GR': return localeEl;
      case 'es-ES': return localeEs;
      case 'fi-FI': return localeFi;
      case 'hu-HU': return localeHu;
      case 'it-IT': return localeIt;
      case 'no-NO': return localeNo;
      case 'pl-PL': return localePl;
      case 'pt-BR': return localePtBr;
      case 'pt-PT': return localePtPt;
      case 'ro-RO': return localeRo;
      case 'sv-SE': return localeSv;
      case 'cs-CZ': return localeCs;
      default: return localeEn;
    }
  }
}
