import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { Subscription } from 'rxjs/Subscription';
import { Observable } from 'rxjs/Observable';
import { TranslateService } from '@ngx-translate/core';

import { ConfigurationFacade } from '../state/facade.configuration';
import { Configuration } from '../models/configuration';
import { ToastService } from '../../shared/services/toast.service';
import { LanguageFacade } from '../../shared/components/language/state/facade.language';
import { Language } from '../../shared/components/language/models/language';

@Component({
  selector: 'app-configuration-general',
  templateUrl: './configuration-general.component.html',
  styleUrls: ['./configuration-general.component.scss']
})
export class ConfigurationGeneralComponent implements OnInit, OnDestroy {
  configuration$: Observable<Configuration>;
  private configuration: Configuration;
  public languageChangedSub: Subscription;
  public configChangedSub: Subscription;
  public languages$: Observable<Language[]>;

  public form: FormGroup;
  public tvdbForm: FormGroup;
  public nameControl = new FormControl('', [Validators.required]);
  public languageControl = new FormControl('en-US', [Validators.required]);
  public tvdbApiKeyControl = new FormControl('', [Validators.required]);

  constructor(
    private configurationFacade: ConfigurationFacade,
    private translate: TranslateService,
    private toaster: ToastService,
    private languageFacade: LanguageFacade) {
    this.languages$ = this.languageFacade.getLanguages();
    this.configuration$ = this.configurationFacade.getConfiguration();

    this.form = new FormGroup({
      name: this.nameControl,
      language: this.languageControl
    });

    this.tvdbForm = new FormGroup({
      tvdbApiKey: this.tvdbApiKeyControl
    });

    this.configChangedSub = this.configuration$.subscribe(config => {
      this.configuration = config;
      this.nameControl.setValue(config.username);
      this.languageControl.setValue(config.language);
      this.tvdbApiKeyControl.setValue(config.tvdbApiKey);
    });

    this.languageChangedSub = this.languageControl.valueChanges
      .subscribe((value => this.languageChanged(value)));
  }

  public saveForm() {
    const config = { ...this.configuration };
    config.language = this.form.get('language').value;
    config.username = this.form.get('name').value;
    this.configurationFacade.updateConfiguration(config);
    this.toaster.pushSuccess('CONFIGURATION.SAVED.GENERAL');
  }

  public saveTvdbForm() {
    const config = { ...this.configuration };
    config.tvdbApiKey = this.tvdbForm.get('tvdbApiKey').value;
    console.log(config.tvdbApiKey);
    this.configurationFacade.updateConfiguration(config);
    this.toaster.pushSuccess('CONFIGURATION.SAVED.GENERAL');
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
