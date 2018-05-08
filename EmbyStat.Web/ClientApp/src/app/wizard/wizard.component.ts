import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { MatStepper } from '@angular/material';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';
import { Subscription } from 'rxjs/Subscription';
import { Router } from '@angular/router';

import { ConfigurationFacade } from '../configuration/state/facade.configuration';
import { EmbyUdpBroadcast } from '../configuration/models/embyUdpBroadcast';
import { Configuration } from '../configuration/models/configuration';
import { EmbyToken } from '../configuration/models/embyToken';
import { EmbyPlugin } from '../plugin/models/embyPlugin';

import { PluginFacade } from '../plugin/state/facade.plugin';

@Component({
  selector: 'app-wizard',
  templateUrl: './wizard.component.html',
  styleUrls: ['./wizard.component.scss']
})

export class WizardComponent implements OnInit, OnDestroy {
  @ViewChild('stepper') private stepper: MatStepper;

  public introFormGroup: FormGroup;
  public nameControl: FormControl = new FormControl('', [Validators.required]);
  public languageControl: FormControl = new FormControl('en', [Validators.required]);

  public embyFormGroup: FormGroup;
  public embyAddressControl: FormControl = new FormControl('', [Validators.required]);
  public embyUsernameControl: FormControl = new FormControl('', [Validators.required]);
  public embyPasswordControl: FormControl = new FormControl('', [Validators.required]);

  public languageChangedSub: Subscription;
  public searchEmbySub: Subscription;
  public configurationSub: Subscription;

  public embyFound = false;
  public embyServerName = '';
  public hidePassword = true;
  public wizardIndex = 0;
  public embyOnline = false;
  public isAdmin = false;
  public username: string;

  private configuration: Configuration;

  constructor(private translate: TranslateService,
    private configurationFacade: ConfigurationFacade,
    private pluginFacade: PluginFacade,
    private router: Router  ) {
    this.introFormGroup = new FormGroup({
      name: this.nameControl,
      language: this.languageControl
    });

    this.embyFormGroup = new FormGroup({
      embyAddress: this.embyAddressControl,
      embyUsername: this.embyUsernameControl,
      embyPassword: this.embyPasswordControl
    });

    this.languageChangedSub = this.languageControl.valueChanges.subscribe((value => this.languageChanged(value)));
    this.configurationSub = this.configurationFacade.configuration$.subscribe(config => this.configuration = config);
  }

  ngOnInit() {
    this.configurationFacade.searchEmby().subscribe((data: EmbyUdpBroadcast) => {
      if (!!data.address) {
        this.embyFound = true;
        this.embyAddressControl.setValue(data.address);
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
      this.username = this.embyFormGroup.get('embyUsername').value;
      const password = this.embyFormGroup.get('embyPassword').value;
      const address = this.embyFormGroup.get('embyAddress').value;
      this.configurationFacade.getToken(this.username, password, address)
        .subscribe((token: EmbyToken) => {
          this.embyOnline = true;
          this.isAdmin = token.isAdmin;
          if (token.isAdmin) {
            const config = { ...this.configuration };
            config.language = this.introFormGroup.get('language').value;
            config.embyUserName = this.username;
            config.username = this.introFormGroup.get('name').value;
            config.embyServerAddress = address;
            config.accessToken = token.token;
            config.wizardFinished = true;
            config.embyUserId = token.id;
            this.configurationFacade.updateConfiguration(config);
          } else {
          }
        }, (err) => {
        });
    }
  }

  public finishWizard() {
    this.router.navigateByUrl('');
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
  }
}
