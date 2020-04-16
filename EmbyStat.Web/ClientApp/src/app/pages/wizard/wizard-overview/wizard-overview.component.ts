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
import { MediaServerUser } from '../../../shared/models/media-server/media-server-user';
import { Settings } from '../../../shared/models/settings/settings';
import { JobService } from '../../../shared/services/job.service';
import { MediaServerService } from '../../../shared/services/media-server.service';
import { SideBarService } from '../../../shared/services/side-bar.service';

@Component({
  selector: 'es-wizard',
  templateUrl: './wizard-overview.component.html',
  styleUrls: ['./wizard-overview.component.scss']
})

export class WizardOverviewComponent implements OnInit, OnDestroy {
  @ViewChild('stepper') private stepper: MatStepper;

  administratorsSub: Subscription;
  administrators: MediaServerUser[];

  introFormGroup: FormGroup;
  nameControl = new FormControl('', [Validators.required]);
  languageControl = new FormControl('en-US', [Validators.required]);

  serverForm: FormGroup;
  serverAddressControl = new FormControl('', [Validators.required]);
  serverPortControl = new FormControl('', [Validators.required]);
  serverProtocolControl = new FormControl('1', [Validators.required]);
  serverApiKeyControl = new FormControl('', [Validators.required]);
  selectedAdministratorControl = new FormControl('');


  exceptionLoggingControl = new FormControl(false);

  private languageChangedSub: Subscription;
  private searchEmbySub: Subscription;
  private settingsSub: Subscription;
  private fireSyncSub: Subscription;
  private serverPortControlChange: Subscription;
  private serverAddressControlChange: Subscription;
  private serverProtocolControlChange: Subscription;
  private selectedAdministratorChange: Subscription;
  private checkUrlNeeded = true;

  serverUrl: string;
  serverFound = CheckBoolean.unChecked;
  serverName = '';
  hidePassword = true;
  wizardIndex = 0;
  serverOnline = CheckBoolean.unChecked;
  apiKeyWorks = CheckBoolean.unChecked;
  apiKey: string;
  selectedProtocol: number;

  adminForm: FormGroup;

  private settings: Settings;
  type: number;
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

    this.serverPortControl.valueChanges.subscribe((value: string) => {
      const url = this.serverAddressControl.value;
      const protocol = this.serverProtocolControl.value;
      this.updateUrl(protocol, url, value);
    });

    this.serverProtocolControl.valueChanges.subscribe((value: number) => {
      const url = this.serverAddressControl.value;
      const port = this.serverPortControl.value;
      this.updateUrl(value, url, port);
    });

    this.serverAddressControl.valueChanges.subscribe((value: string) => {
      const port = this.serverPortControl.value;
      const protocol = this.serverProtocolControl.value;
      this.updateUrl(protocol, value, port);
    });

    this.serverForm = new FormGroup({
      serverAddress: this.serverAddressControl,
      serverPort: this.serverPortControl,
      serverProtocol: this.serverProtocolControl,
      serverApiKey: this.serverApiKeyControl,
    });

    this.adminForm = new FormGroup({
      selectedAdministrator: this.selectedAdministratorControl
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
    this.serverProtocolControl.setValue(0);
  }

  ngOnInit(): void {
    this.languages$ = this.settingsFacade.getLanguages();
  }

  private languageChanged(value: string): void {
    this.translate.use(value);
  }

  private updateUrl(protocol: number, url: string, port: string): void {
    this.serverUrl = (protocol === 0 ? 'https://' : 'http://') + url + ':' + port;
  }

  selectType(type: string): void {
    this.type = type === 'emby' ? 0 : 1;
    this.typeText = MediaServerTypeSelector.getServerTypeString(this.type);
    this.stepper.selectedIndex = 2;
  }

  stepperPageChanged(event): void {
    if (event.selectedIndex === 2) {
      this.serverFound = CheckBoolean.unChecked;
      this.serverApiKeyControl.setValue('');

      console.log(this.type);
      this.mediaServerService.searchMediaServer(this.type).subscribe((data: MediaServerUdpBroadcast) => {
        if (data.address) {
          this.serverFound = CheckBoolean.true;
          this.serverAddressControl.setValue(data.address);
          this.serverPortControl.setValue(data.port);
          this.serverProtocolControl.setValue(data.protocol);
          this.serverName = data.name;
        } else {
          this.serverFound = CheckBoolean.false;
        }
      },
      err => {
        this.serverFound = CheckBoolean.false;
      }
      );
    } else if (event.selectedIndex === 3) {
      this.apiKey = this.serverApiKeyControl.value;

      const login = new MediaServerLogin(this.apiKey, this.serverUrl);

      this.serverOnline = CheckBoolean.busy;
      this.apiKeyWorks = CheckBoolean.busy;

      this.mediaServerService.pingEmby(this.serverUrl).subscribe((response: boolean) => {
        this.serverOnline = response ? CheckBoolean.true : CheckBoolean.false;
        if (response) {
          this.mediaServerService.testApiKey(login)
            .subscribe((result: boolean) => {
              this.apiKeyWorks = result ? CheckBoolean.true : CheckBoolean.false;
              if (result) {
                this.saveMediaServerDetails();
              }
            }, (err) => {
              this.apiKeyWorks = CheckBoolean.false;
            });
        }
      },
      err => {
        this.serverOnline = CheckBoolean.false;
        this.apiKeyWorks = CheckBoolean.false;
      });
    } else if (event.selectedIndex === 4) {
      if (this.type === 1) {
        this.administratorsSub = this.mediaServerService.getAdministrators().subscribe((admins: MediaServerUser[]) => {
          this.administrators = admins;
          if (admins.length > 0) {
            this.selectedAdministratorControl.setValue(admins[0].id);
          }
        });
        this.selectedAdministratorChange = this.selectedAdministratorControl.valueChanges.subscribe(() => {
          this.saveMediaServerDetails();
        });
      }
    }
  }

  private saveMediaServerDetails(): void {
    const address = this.serverAddressControl.value;
    const port = +this.serverPortControl.value;
    const protocol = this.serverProtocolControl.value;

    const settings = { ...this.settings };
    const server = { ...this.settings.mediaServer };
    settings.language = this.languageControl.value;
    settings.username = this.nameControl.value;
    settings.wizardFinished = true;
    settings.enableRollbarLogging = this.exceptionLoggingControl.value;
    server.serverAddress = address;
    server.apiKey = this.apiKey;
    server.serverPort = port;
    server.serverProtocol = protocol;
    server.serverType = this.type;
    server.userId = this.selectedAdministratorControl.value;
    settings.mediaServer = server;
    this.settingsFacade.updateSettings(settings);
  }

  finishWizard(): void {
    this.jobService.fireJob('41e0bf22-1e6b-4f5d-90be-ec966f746a2f').subscribe().unsubscribe();
    this.sideBarService.openMenu();
    this.router.navigate(['']);
  }

  finishWizardAndStartSync(): void {
    this.jobService.fireJob('41e0bf22-1e6b-4f5d-90be-ec966f746a2f').subscribe().unsubscribe();
    this.jobService.fireJob('be68900b-ee1d-41ef-b12f-60ef3106052e').subscribe().unsubscribe();
    this.sideBarService.openMenu();
    this.router.navigate(['/jobs']);
  }

  getPage(): string {
    return MediaServerTypeSelector.getServerApiPage(this.type);
  }

  ngOnDestroy(): void {
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

    if (this.serverPortControlChange !== undefined) {
      this.serverPortControlChange.unsubscribe();
    }

    if (this.serverProtocolControlChange !== undefined) {
      this.serverProtocolControlChange.unsubscribe();
    }

    if (this.serverAddressControlChange !== undefined) {
      this.serverAddressControlChange.unsubscribe();
    }

    if (this.selectedAdministratorChange !== undefined) {
      this.selectedAdministratorChange.unsubscribe();
    }

    if (this.administratorsSub !== undefined) {
      this.administratorsSub.unsubscribe();
    }
  }
}
