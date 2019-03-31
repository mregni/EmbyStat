import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { Subscription ,  Observable } from 'rxjs';
import { TranslateService } from '@ngx-translate/core';

import { SettingsFacade } from '../state/facade.settings';
import { Settings } from '../models/settings';
import { ToastService } from '../../shared/services/toast.service';
import { LanguageFacade } from '../../shared/components/language/state/facade.language';
import { Language } from '../../shared/components/language/models/language';
@Component({
  selector: 'app-settings-general',
  templateUrl: './settings-general.component.html',
  styleUrls: ['./settings-general.component.scss']
})
export class SettingsGeneralComponent implements OnInit, OnDestroy {
  settings$: Observable<Settings>;
  private settings: Settings;

  languageChangedSub: Subscription;
  settingsChangedSub: Subscription;
  languages$: Observable<Language[]>;

  form: FormGroup;
  tvdbForm: FormGroup;
  exceptionLoggingForm: FormGroup;

  nameControl = new FormControl('', [Validators.required]);
  languageControl = new FormControl('en-US', [Validators.required]);
  tvdbApiKeyControl = new FormControl('', [Validators.required]);
  exceptionLoggingControl = new FormControl(false);

  constructor(
    private settingsFacade: SettingsFacade,
    private translate: TranslateService,
    private toaster: ToastService,
    private languageFacade: LanguageFacade) {
    this.languages$ = this.languageFacade.getLanguages();
    this.settings$ = this.settingsFacade.getSettings();

    this.form = new FormGroup({
      name: this.nameControl,
      language: this.languageControl
    });

    this.tvdbForm = new FormGroup({
      tvdbApiKey: this.tvdbApiKeyControl
    });

    this.exceptionLoggingForm = new FormGroup({
      exceptionLogging: this.exceptionLoggingControl
  });

    this.settingsChangedSub = this.settings$.subscribe((settings: Settings) => {
      this.settings = settings;
      this.nameControl.setValue(settings.username);
      this.languageControl.setValue(settings.language);
      this.tvdbApiKeyControl.setValue(settings.tvdb.apiKey);
      this.exceptionLoggingControl.setValue(settings.enableRollbarLogging);
    });

    this.languageChangedSub = this.languageControl.valueChanges
      .subscribe((value => this.languageChanged(value)));
  }

  saveForm() {
    const settings = { ...this.settings };
    settings.language = this.languageControl.value;
    settings.username = this.nameControl.value;
    this.settingsFacade.updateSettings(settings);
    this.toaster.pushSuccess('SETTINGS.SAVED.GENERAL');
  }

  saveTvdbForm() {
    const settings = { ...this.settings };
    const tvdb = { ...this.settings.tvdb };
    tvdb.apiKey = this.tvdbApiKeyControl.value;
    settings.tvdb = tvdb;
    this.settingsFacade.updateSettings(settings);
    this.toaster.pushSuccess('SETTINGS.SAVED.GENERAL');
  }

  saveExceptionLoggingForm() {
    const settings = { ...this.settings };
    settings.enableRollbarLogging = this.exceptionLoggingControl.value;
    this.settingsFacade.updateSettings(settings);
    this.toaster.pushSuccess('SETTINGS.SAVED.GENERAL');
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

    if (this.settingsChangedSub !== undefined) {
      this.settingsChangedSub.unsubscribe();
    }
  }
}
