import { Observable } from 'rxjs';

import { Component, Input, OnChanges } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';

import { SettingsFacade } from '../../../../shared/facades/settings.facade';
import { Language } from '../../../../shared/models/language';
import { Settings } from '../../../../shared/models/settings/settings';
import { ToastService } from '../../../../shared/services/toast.service';

@Component({
  selector: 'es-settings-general',
  templateUrl: './settings-general.component.html',
  styleUrls: ['./settings-general.component.scss']
})
export class SettingsGeneralComponent implements OnChanges {
  @Input() settings: Settings;

  isSaving = false;
  languages$: Observable<Language[]>;

  generalForm: FormGroup;
  nameControl = new FormControl('', [Validators.required]);
  languageControl = new FormControl('', [Validators.required]);
  exceptionLoggingControl = new FormControl('');

  tvdbForm: FormGroup;
  tvdbKeyControl = new FormControl('');

  constructor(
    private readonly settingsFacade: SettingsFacade,
    private readonly toastService: ToastService) {
    this.languages$ = this.settingsFacade.getLanguages();

    this.generalForm = new FormGroup({
      name: this.nameControl,
      language: this.languageControl,
      exceptionLogging: this.exceptionLoggingControl
    });

    this.tvdbForm = new FormGroup({
      tvdbKey: this.tvdbKeyControl
    });
  }

  ngOnChanges(): void {
    if (this.settings !== undefined) {
      this.nameControl.setValue(this.settings.username);
      this.languageControl.setValue(this.settings.language);
      this.exceptionLoggingControl.setValue(this.settings.enableRollbarLogging);
      this.tvdbKeyControl.setValue(this.settings.tvdb.apiKey);
    }
  }

  saveGeneralForm(): void {
    if (this.checkForm(this.generalForm)) {
      this.isSaving = true;
      this.generalForm.disable();
      this.markFormAsUntouched(this.tvdbForm);

      const settings = { ...this.settings };
      settings.language = this.languageControl.value;
      settings.username = this.nameControl.value;
      settings.enableRollbarLogging = this.exceptionLoggingControl.value;

      this.saveSettings(settings, 'SETTINGS.SAVED.GENERAL');
      this.generalForm.enable();
    }
  }

  saveTvdbForm(): void {
    if (this.checkForm(this.tvdbForm)) {
      this.isSaving = true;
      this.tvdbForm.disable();
      this.markFormAsUntouched(this.generalForm);

      const settings = { ...this.settings };
      const tvdb = { ...this.settings.tvdb };
      tvdb.apiKey = this.tvdbKeyControl.value;
      settings.tvdb = tvdb;
      this.saveSettings(settings, 'SETTINGS.SAVED.TVDB');
      this.tvdbForm.enable();
    }
  }

  private saveSettings(settings: Settings, confirmMessage: string): void {
    this.settingsFacade.updateSettings(settings);
    this.toastService.showSuccess(confirmMessage);
    this.isSaving = false;
  }

  private checkForm(form: FormGroup): boolean {
    for (const i of Object.keys(form.controls)) {
      form.controls[i].markAsTouched();
      form.controls[i].updateValueAndValidity();
    }

    return form.valid;
  }

  private markFormAsUntouched(form: FormGroup): void {
    for (const i of Object.keys(form.controls)) {
      form.controls[i].markAsUntouched();
    }
  }
}
