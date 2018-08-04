import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { MatStepper } from '@angular/material';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';
import { Subscription } from 'rxjs/Subscription';
import { Observable } from 'rxjs/Observable';
import { Router } from '@angular/router';

import { ConfigurationFacade } from '../../configuration/state/facade.configuration';
import { EmbyUdpBroadcast } from '../../shared/models/emby/embyUdpBroadcast';
import { Configuration } from '../../configuration/models/configuration';
import { EmbyToken } from '../../shared/models/emby//embyToken';
import { Language } from '../../shared/components/language/models/language';
import { LanguageFacade } from '../../shared/components/language/state/facade.language';

import { PluginFacade } from '../../plugin/state/facade.plugin';
import { WizardStateService } from '../services/wizard-state.service';

import { TaskFacade } from '../../task/state/facade.task';
import { Task } from '../../task/models/task';

@Component({
  selector: 'app-wizard',
  templateUrl: './wizard-overview.component.html',
  styleUrls: ['./wizard-overview.component.scss']
})

export class WizardOverviewComponent implements OnInit, OnDestroy {
  @ViewChild('stepper') private stepper: MatStepper;

  public introFormGroup: FormGroup;
  public nameControl = new FormControl('', [Validators.required]);
  public languageControl = new FormControl('en-US', [Validators.required]);

  public embyFormGroup: FormGroup;
  public embyAddressControl = new FormControl('', [Validators.required]);
  public embyPortControl = new FormControl('', [Validators.required]);
  public embyProtocolControl = new FormControl('', [Validators.required]);
  public embyUsernameControl = new FormControl('', [Validators.required]);
  public embyPasswordControl = new FormControl('', [Validators.required]);

  private languageChangedSub: Subscription;
  private searchEmbySub: Subscription;
  private configurationSub: Subscription;
  private getTasksSub: Subscription;

  public embyFound = false;
  public embyServerName = '';
  public hidePassword = true;
  public wizardIndex = 0;
  public embyOnline = false;
  public isAdmin = false;
  public username: string;
  public selectedProtocol: number;

  private configuration: Configuration;
  public languages$: Observable<Language[]>;

  private tasks: Task[];

  constructor(private translate: TranslateService,
    private configurationFacade: ConfigurationFacade,
    private pluginFacade: PluginFacade,
    private languageFacade: LanguageFacade,
    private wizardStateService: WizardStateService,
    private taskFacade: TaskFacade,
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
    this.configurationSub = this.configurationFacade.configuration$.subscribe(config => this.configuration = config);
    this.getTasksSub = this.taskFacade.getTasks().subscribe((result: Task[]) => this.tasks = result);

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

  public rescan() {
    this.configurationFacade.fireSmallEmbySync();
  }

  public stepperPageChanged(event) {
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

  public finishWizard() {
    this.wizardStateService.changeState(true);
  }

  public finishWizardAndStartSync() {
    const task = this.tasks.find(x => x.name === 'TASKS.MEDIASYNCTITLE');
    this.taskFacade.fireTask(task.id);
    this.wizardStateService.changeState(true);
    this.router.navigate(['/task']);
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

    if (this.getTasksSub !== undefined) {
      this.getTasksSub.unsubscribe();
    }
  }
}
