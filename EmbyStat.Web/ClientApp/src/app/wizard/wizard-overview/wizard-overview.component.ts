import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { MatStepper } from '@angular/material';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';
import { Subscription } from 'rxjs/Subscription';
import { Observable } from 'rxjs/Observable';
import { Router } from '@angular/router';

import { ConfigurationFacade } from '../../configuration/state/facade.configuration';
import { EmbyUdpBroadcast } from '../../shared/models/emby/emby-udp-broadcast';
import { Configuration } from '../../configuration/models/configuration';
import { EmbyToken } from '../../shared/models/emby/emby-token';
import { Language } from '../../shared/components/language/models/language';
import { LanguageFacade } from '../../shared/components/language/state/facade.language';

import { PluginService } from '../../plugin/service/plugin.service';
import { SideBarService } from '../../shared/services/side-bar.service';

import { JobService } from '../../jobs/service/job.service';

@Component({
  selector: 'app-wizard',
  templateUrl: './wizard-overview.component.html',
  styleUrls: ['./wizard-overview.component.scss']
})

export class WizardOverviewComponent implements OnInit, OnDestroy {
  @ViewChild('stepper') private stepper: MatStepper;

  introFormGroup: FormGroup;
  nameControl = new FormControl('', [Validators.required]);
  languageControl = new FormControl('en-US', [Validators.required]);

  embyFormGroup: FormGroup;
  embyAddressControl = new FormControl('', [Validators.required]);
  embyPortControl = new FormControl('', [Validators.required]);
  embyProtocolControl = new FormControl('', [Validators.required]);
  embyUsernameControl = new FormControl('', [Validators.required]);
  embyPasswordControl = new FormControl('', [Validators.required]);

  private languageChangedSub: Subscription;
  private searchEmbySub: Subscription;
  private configurationSub: Subscription;
  private fireSyncSub: Subscription;
  private checkUrlNeeded = true;

  embyFound = false;
  embyServerName = '';
  hidePassword = true;
  wizardIndex = 0;
  embyOnline = false;
  isAdmin = false;
  username: string;
  selectedProtocol: number;

  private configuration: Configuration;
  languages$: Observable<Language[]>;


  constructor(private translate: TranslateService,
    private configurationFacade: ConfigurationFacade,
    private pluginService: PluginService,
    private languageFacade: LanguageFacade,
    private sideBarService: SideBarService,
    private jobService: JobService,
    private router: Router) {
    this.introFormGroup = new FormGroup({
      name: this.nameControl,
      language: this.languageControl
    });

    this.embyFormGroup = new FormGroup({
      embyAddress: this.embyAddressControl,
      embyPort: this.embyPortControl,
      embyProtocol: this.embyProtocolControl,
      embyUsername: this.embyUsernameControl,
      embyPassword: this.embyPasswordControl
    });

    this.languageChangedSub = this.languageControl.valueChanges.subscribe((value => this.languageChanged(value)));
    this.configurationSub = this.configurationFacade.configuration$.subscribe(config => {
      this.configuration = config;
      if (config.wizardFinished && this.checkUrlNeeded) {
        this.router.navigate(['']);
      }

      this.checkUrlNeeded = false;
      this.sideBarService.closeMenu();
    });
    this.embyProtocolControl.setValue(0);
  }

  ngOnInit() {
    this.languages$ = this.languageFacade.getLanguages();

    this.configurationFacade.searchEmby().subscribe((data: EmbyUdpBroadcast) => {
      if (!!data.address) {
        this.embyFound = true;
        this.embyAddressControl.setValue(data.address);
        this.embyPortControl.setValue(data.port);
        this.embyProtocolControl.setValue(data.protocol);
        this.embyServerName = data.name;
      }
    });
  }

  private languageChanged(value: string): void {
    this.translate.use(value);
  }

  stepperPageChanged(event) {
    if (event.selectedIndex === 2) {
      this.username = this.embyUsernameControl.value;
      const password = this.embyPasswordControl.value;
      const address = this.embyAddressControl.value;
      const port = this.embyPortControl.value;
      const protocol = this.embyProtocolControl.value;

      const url = (protocol === 0 ? 'http://' : 'https://') + address + ':' + port;
      this.configurationFacade.getToken(this.username, password, url)
        .subscribe((token: EmbyToken) => {
          this.embyOnline = true;
          this.isAdmin = token.isAdmin;
          if (token.isAdmin) {
            const config = { ...this.configuration };
            config.language = this.languageControl.value;
            config.embyUserName = this.username;
            config.username = this.nameControl.value;
            config.embyServerAddress = address;
            config.accessToken = token.token;
            config.wizardFinished = true;
            config.embyUserId = token.id;
            config.embyServerPort = port;
            config.embyServerProtocol = protocol;
            this.configurationFacade.updateConfiguration(config);
          }
        }, (err) => {
        });
    }
  }

  finishWizard() {
    this.jobService.fireSmallSyncJob();
    this.sideBarService.openMenu();
    this.router.navigate(['']);
  }

  finishWizardAndStartSync() {
    this.jobService.fireSmallSyncJob();
    this.jobService.fireMediaSyncJob();
    this.sideBarService.openMenu();
    this.router.navigate(['/jobs']);
  }

  ngOnDestroy() {
    if (this.languageChangedSub !== undefined) {
      this.languageChangedSub.unsubscribe();
    }

    if (this.searchEmbySub !== undefined) {
      this.searchEmbySub.unsubscribe();
    }

    if (this.configurationSub !== undefined) {
      this.configurationSub.unsubscribe();
    }

    if (this.fireSyncSub !== undefined) {
      this.fireSyncSub.unsubscribe();
    }
  }
}
