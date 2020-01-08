import { Observable, Subscription } from 'rxjs';
import { MediaServerTypeSelector } from 'src/app/shared/helpers/media-server-type-selector';

import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MatStepper } from '@angular/material/stepper';
import { Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';

import { CheckBoolean } from '../../../shared/enums/check-boolean-enum';
import { SettingsFacade } from '../../../shared/facades/settings.facade';
import { Language } from '../../../shared/models/language';
import { MediaServerLogin } from '../../../shared/models/media-server/media-server-login';
import {
    MediaServerUdpBroadcast
} from '../../../shared/models/media-server/media-server-udp-broadcast';
import { Settings } from '../../../shared/models/settings/settings';
import { JobService } from '../../../shared/services/job.service';
import { MediaServerService } from '../../../shared/services/media-server.service';
import { SideBarService } from '../../../shared/services/side-bar.service';

@Component({
  selector: 'app-wizard',
  templateUrl: './wizard-overview.component.html',
  styleUrls: ['./wizard-overview.component.scss']
})

export class WizardOverviewComponent implements OnInit, OnDestroy {
  @ViewChild('stepper', {static: false}) private stepper: MatStepper;

  introFormGroup: FormGroup;
  nameControl = new FormControl('', [Validators.required]);
  languageControl = new FormControl('en-US', [Validators.required]);

  embyForm: FormGroup;
  embyAddressControl = new FormControl('', [Validators.required]);
  embyPortControl = new FormControl('', [Validators.required]);
  embyProtocolControl = new FormControl('1', [Validators.required]);
  embyApiKeyControl = new FormControl('', [Validators.required]);

  exceptionLoggingControl = new FormControl(false);

  private languageChangedSub: Subscription;
  private searchEmbySub: Subscription;
  private settingsSub: Subscription;
  private fireSyncSub: Subscription;
  private embyPortControlChange: Subscription;
  private embyAddressControlChange: Subscription;
  private embyProtocolControlChange: Subscription;
  private checkUrlNeeded = true;

  embyUrl: string;
  embyFound = CheckBoolean.unChecked;
  embyServerName = '';
  hidePassword = true;
  wizardIndex = 0;
  embyOnline = CheckBoolean.unChecked;
  apiKeyWorks = CheckBoolean.unChecked;
  apiKey: string;
  selectedProtocol: number;

  private settings: Settings;
  private type: number;
  typeText: string;
  languages$: Observable<Language[]>;

  constructor(private translate: TranslateService,
    private settingsFacade: SettingsFacade,
    private mediaServerService: MediaServerService,
    private sideBarService: SideBarService,
    private jobService: JobService,
    private router: Router) {
    this.introFormGroup = new FormGroup({
      name: this.nameControl,
      language: this.languageControl,
      exceptionLogging: this.exceptionLoggingControl
    });

    this.embyPortControl.valueChanges.subscribe((value: string) => {
      const url = this.embyAddressControl.value;
      const protocol = this.embyProtocolControl.value;
      this.updateUrl(protocol, url, value);
    });

    this.embyProtocolControl.valueChanges.subscribe((value: number) => {
      const url = this.embyAddressControl.value;
      const port = this.embyPortControl.value;
      this.updateUrl(value, url, port);
    });

    this.embyAddressControl.valueChanges.subscribe((value: string) => {
      const port = this.embyPortControl.value;
      const protocol = this.embyProtocolControl.value;
      this.updateUrl(protocol, value, port);
    });

    this.embyForm = new FormGroup({
      embyAddress: this.embyAddressControl,
      embyPort: this.embyPortControl,
      embyProtocol: this.embyProtocolControl,
      embyApiKey: this.embyApiKeyControl,
    });

    this.languageChangedSub = this.languageControl.valueChanges.subscribe((value => this.languageChanged(value)));
    this.settingsSub = this.settingsFacade.settings$.subscribe(config => {
      this.settings = config;
      if (config.wizardFinished && this.checkUrlNeeded) {
        this.router.navigate(['']);
      }

      this.checkUrlNeeded = false;
      this.sideBarService.closeMenu();
    });
    this.embyProtocolControl.setValue(0);
  }

  ngOnInit() {
    this.languages$ = this.settingsFacade.getLanguages();

    this.mediaServerService.searchEmby().subscribe((data: MediaServerUdpBroadcast) => {
    if (!!data.address) {
        this.embyFound = CheckBoolean.true;
        this.embyAddressControl.setValue(data.address);
        this.embyPortControl.setValue(data.port);
        this.embyProtocolControl.setValue(data.protocol);
        this.embyServerName = data.name;
      } else {
        this.embyFound = CheckBoolean.false;
      }
    },
      err => {
        this.embyFound = CheckBoolean.false;
      });
  }

  private languageChanged(value: string): void {
    this.translate.use(value);
  }

  private updateUrl(protocol: number, url: string, port: string) {
    this.embyUrl = (protocol === 0 ? 'https://' : 'http://') + url + ':' + port;
  }

  selectType(type: string) {
    this.type = type === 'emby' ? 0 : 1;
    this.typeText = MediaServerTypeSelector.getServerTypeString(this.type);
    this.stepper.selectedIndex = 2;
  }

  stepperPageChanged(event) {
    if (event.selectedIndex === 2) {
      this.embyFound = CheckBoolean.unChecked;
      this.embyApiKeyControl.setValue('');
    }

    if (event.selectedIndex === 3) {
      this.apiKey = this.embyApiKeyControl.value;
      const address = this.embyAddressControl.value;
      const port = this.embyPortControl.value;
      const protocol = this.embyProtocolControl.value;

      const login = new MediaServerLogin(this.apiKey, this.embyUrl);

      this.embyOnline = CheckBoolean.busy;
      this.apiKeyWorks = CheckBoolean.busy;

      this.mediaServerService.pingEmby(this.embyUrl).subscribe((response: boolean) => {
        this.embyOnline = response ? CheckBoolean.true : CheckBoolean.false;
        if (response) {
          this.mediaServerService.testApiKey(login)
            .subscribe((result: boolean) => {
              this.apiKeyWorks = result ? CheckBoolean.true : CheckBoolean.false;
              if (result) {
                const settings = { ...this.settings };
                const emby = { ...this.settings.mediaServer };
                settings.language = this.languageControl.value;
                settings.username = this.nameControl.value;
                settings.wizardFinished = true;
                settings.enableRollbarLogging = this.exceptionLoggingControl.value;
                emby.serverAddress = address;
                emby.apiKey = this.apiKey;
                emby.serverPort = port;
                emby.serverProtocol = protocol;
                emby.serverType = this.type;
                settings.mediaServer = emby;
                this.settingsFacade.updateSettings(settings);
              }
            }, (err) => {
              this.apiKeyWorks = CheckBoolean.false;
            });
        }
      },
        err => {
          this.embyOnline = CheckBoolean.false;
          this.apiKeyWorks = CheckBoolean.false;
        });
    }
  }

  finishWizard() {
    this.jobService.fireJob('41e0bf22-1e6b-4f5d-90be-ec966f746a2f').subscribe().unsubscribe();
    this.sideBarService.openMenu();
    this.router.navigate(['']);
  }

  finishWizardAndStartSync() {
    this.jobService.fireJob('41e0bf22-1e6b-4f5d-90be-ec966f746a2f').subscribe().unsubscribe();
    this.jobService.fireJob('be68900b-ee1d-41ef-b12f-60ef3106052e').subscribe().unsubscribe();
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

    if (this.settingsSub !== undefined) {
      this.settingsSub.unsubscribe();
    }

    if (this.fireSyncSub !== undefined) {
      this.fireSyncSub.unsubscribe();
    }

    if (this.embyPortControlChange !== undefined) {
      this.embyPortControlChange.unsubscribe();
    }

    if (this.embyProtocolControlChange !== undefined) {
      this.embyProtocolControlChange.unsubscribe();
    }

    if (this.embyAddressControlChange !== undefined) {
      this.embyAddressControlChange.unsubscribe();
    }
  }
}
