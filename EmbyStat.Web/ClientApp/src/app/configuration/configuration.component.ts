import { Component, OnInit, OnDestroy } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';
import { Subscription } from 'rxjs/Subscription';

import { ConfigurationFacade } from './state/facade.configuration';
import { Configuration } from './models/configuration';

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

  public languageChangedSub: Subscription;
  public configChangedSub: Subscription;

  constructor(private configurationFacade: ConfigurationFacade, private translate: TranslateService) {
    this.configuration$ = this.configurationFacade.getConfiguration();

    this.introFormGroup = new FormGroup({
      name: this.nameControl,
      language: this.languageControl
    });

    this.configChangedSub = this.configuration$.subscribe(config => {
      this.configuration = config;
      this.introFormGroup.setValue({ name: config.username, language: config.language });
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
    this.configurationFacade.updateConfiguration(config);
  }

  public resetIntroForm() {
    this.introFormGroup.setValue({ name: this.configuration.username, language: this.configuration.language });
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
