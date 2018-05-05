import { Component, OnInit, OnDestroy } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';
import { Subscription } from 'rxjs/Subscription';

import { ConfigurationFacade } from './state/facade.configuration';
import { Configuration } from './models/configuration';
import { EmbyToken } from './models/embyToken';

@Component({
  selector: 'app-configuration',
  templateUrl: './configuration.component.html',
  styleUrls: ['./configuration.component.scss']
})
export class ConfigurationComponent implements OnInit, OnDestroy {
  configuration$: Observable<Configuration>;
  private configuration: Configuration;

  public introFormGroup: FormGroup;
  public nameControl: FormControl = new FormControl('', [Validators.required]);
  public languageControl: FormControl = new FormControl('en', [Validators.required]);

  public embyFormGroup: FormGroup;
  public embyAddressControl: FormControl = new FormControl('', [Validators.required]);
  public embyUsernameControl: FormControl = new FormControl('', [Validators.required]);
  public embyPasswordControl: FormControl = new FormControl('', [Validators.required]);

  public languageChangedSub: Subscription;
  public configChangedSub: Subscription;

  public hidePassword: boolean = true; 

  constructor(private configurationFacade: ConfigurationFacade, private translate: TranslateService) {
    this.configuration$ = this.configurationFacade.getConfiguration();

    this.introFormGroup = new FormGroup({
      name: this.nameControl,
      language: this.languageControl
    });

    this.embyFormGroup = new FormGroup({
      embyAddress: this.embyAddressControl,
      embyUsername: this.embyUsernameControl,
      embyPassword: this.embyPasswordControl
    });

    this.configChangedSub = this.configuration$.subscribe(config => {
      this.configuration = config;
      this.introFormGroup.setValue({ name: config.username, language: config.language });
      this.embyFormGroup.setValue({ embyUsername: config.embyUserName, embyAddress: config.embyServerAddress, embyPassword: ""});
    });

    this.languageChangedSub = this.languageControl.valueChanges.subscribe((value => this.languageChanged(value)));
  }

  public saveIntroForm() {
    var config = { ...this.configuration };
    config.language = this.introFormGroup.get('language').value;
    config.embyUserName = this.configuration.embyUserName;
    config.username = this.introFormGroup.get('name').value;
    config.embyServerAddress = this.configuration.embyServerAddress;
    config.accessToken = this.configuration.accessToken;
    config.wizardFinished = true;
    config.Id = this.configuration.Id;
    this.configurationFacade.updateConfiguration(config);
  }

  public saveEmbyForm() {
    var username = this.embyFormGroup.get('embyUsername').value;
    var password = this.embyFormGroup.get('embyPassword').value;
    var address = this.embyFormGroup.get('embyAddress').value;
    this.configurationFacade.getToken(username, password, address)
      .subscribe((token: EmbyToken) => {
        if (token.isAdmin) {
          var config = { ...this.configuration };
          config.language = this.configuration.language;
          config.embyUserName = username;
          config.username = this.configuration.username;
          config.embyServerAddress = address;
          config.accessToken = token.token;
          config.wizardFinished = this.configuration.wizardFinished;
          config.Id = token.id;
          this.configurationFacade.updateConfiguration(config);
        }
      });
  }

  private languageChanged(value: string): void {
    this.translate.use(value);
  }

  ngOnInit() {

  }

  ngOnDestroy() {
    if (this.languageChangedSub !== undefined) {
      this.languageChangedSub.unsubscribe();
    }

    if (this.configChangedSub !== undefined) {
      this.configChangedSub.unsubscribe();
    }
  }
}
