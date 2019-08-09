import { Observable, Subscription } from 'rxjs';

import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MatStepper } from '@angular/material';
import { Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';

import { CheckBoolean } from '../../../shared/enums/check-boolean-enum';
import { SettingsFacade } from '../../../shared/facades/settings.facade';
import { EmbyLogin } from '../../../shared/models/emby/emby-login';
import { EmbyToken } from '../../../shared/models/emby/emby-token';
import { EmbyUdpBroadcast } from '../../../shared/models/emby/emby-udp-broadcast';
import { Language } from '../../../shared/models/language';
import { Settings } from '../../../shared/models/settings/settings';
import { EmbyService } from '../../../shared/services/emby.service';
import { JobService } from '../../../shared/services/job.service';
import { SideBarService } from '../../../shared/services/side-bar.service';

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

  embyForm: FormGroup;
  embyAddressControl = new FormControl('', [Validators.required]);
  embyPortControl = new FormControl('', [Validators.required]);
  embyProtocolControl = new FormControl('1', [Validators.required]);
  embyUsernameControl = new FormControl('', [Validators.required]);
  embyPasswordControl = new FormControl('', [Validators.required]);
  exceptionLoggingControl = new FormControl(false);

  private languageChangedSub: Subscription;
  private searchEmbySub: Subscription;
  private settingsSub: Subscription;
  private fireSyncSub: Subscription;
  private checkUrlNeeded = true;

  embyFound = CheckBoolean.unChecked;
  embyServerName = '';
  hidePassword = true;
  wizardIndex = 0;
  embyOnline = CheckBoolean.unChecked;
  isAdmin = CheckBoolean.unChecked;
  username: string;
  selectedProtocol: number;

  private settings: Settings;
  languages$: Observable<Language[]>;

  constructor(private translate: TranslateService,
    private settingsFacade: SettingsFacade,
    private embyService: EmbyService,
    private sideBarService: SideBarService,
    private jobService: JobService,
    private router: Router) {
    this.introFormGroup = new FormGroup({
      name: this.nameControl,
      language: this.languageControl,
      exceptionLogging: this.exceptionLoggingControl
    });

    this.embyForm = new FormGroup({
      embyAddress: this.embyAddressControl,
      embyPort: this.embyPortControl,
      embyProtocol: this.embyProtocolControl,
      embyUsername: this.embyUsernameControl,
      embyPassword: this.embyPasswordControl
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

    this.embyService.searchEmby().subscribe((data: EmbyUdpBroadcast) => {
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

  stepperPageChanged(event) {
    if (event.selectedIndex === 2) {
      this.username = this.embyUsernameControl.value;
      const password = this.embyPasswordControl.value;
      const address = this.embyAddressControl.value;
      const port = this.embyPortControl.value;
      const protocol = this.embyProtocolControl.value;

      const url = (protocol === 0 ? 'https://' : 'http://') + address + ':' + port;
      const login = new EmbyLogin(this.username, password, url);

      this.embyOnline = CheckBoolean.busy;
      this.isAdmin = CheckBoolean.busy;

      this.embyService.pingEmby(url).subscribe((response: boolean) => {
        this.embyOnline = response ? CheckBoolean.true : CheckBoolean.false;
        if (response) {
          this.embyService.getEmbyToken(login)
            .subscribe((token: EmbyToken) => {
              this.isAdmin = token.isAdmin;
              if (token.isAdmin) {
                const settings = { ...this.settings };
                const emby = { ...this.settings.emby };
                settings.language = this.languageControl.value;
                settings.username = this.nameControl.value;
                settings.wizardFinished = true;
                settings.enableRollbarLogging = this.exceptionLoggingControl.value;
                emby.serverAddress = address;
                emby.userName = this.username;
                emby.accessToken = token.token;
                emby.userId = token.id;
                emby.serverPort = port;
                emby.serverProtocol = protocol;
                settings.emby = emby;
                this.settingsFacade.updateSettings(settings);
              }
            }, (err) => {
              this.isAdmin = CheckBoolean.false;
            });
        }
      },
        err => {
          this.embyOnline = CheckBoolean.false;
          this.isAdmin = CheckBoolean.false;
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
  }
}
