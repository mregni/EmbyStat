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
import { EmbyPluginStore } from '../plugin/models/embyPluginStore';

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
  public pluginSub: Subscription;

  public embyFound: boolean = false;
  public embyServerName: string = "";
  public hidePassword: boolean = true;
  public wizardIndex: number = 0;
  public embyOnline: boolean = false;
  public isAdmin: boolean = false;
  public pluginInstalled: boolean = false;
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
    this.pluginSub = this.pluginFacade.plugins$.subscribe(plugins => { console.log(plugins);
      this.pluginsLoaded(plugins);
    });
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

  private pluginsLoaded(plugins: EmbyPluginStore) {
    this.pluginInstalled = plugins.list.some(plugin => plugin.name === "Statistics");
  }

  public rescan() {
    this.configurationFacade.fireSmallEmbySync();
  }

  public stepperPageChanged(event) {
    if (event.selectedIndex === 2) {
      this.username = this.embyFormGroup.get('embyUsername').value;
      var password = this.embyFormGroup.get('embyPassword').value;
      var address = this.embyFormGroup.get('embyAddress').value;
      this.configurationFacade.getToken(this.username, password, address)
        .subscribe((token: EmbyToken) => {
          this.embyOnline = true;
          this.isAdmin = token.isAdmin;
          if (token.isAdmin) {
            var config = { ...this.configuration };
            config.language = this.introFormGroup.get('language').value;
            config.embyUserName = this.username;
            config.username = this.introFormGroup.get('name').value;
            config.embyServerAddress = address;
            config.accessToken = token.token;
            config.wizardFinished = true;
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

    if (this.pluginSub !== undefined) {
      this.pluginSub.unsubscribe();
    }
  }
}
