import { Observable, Subscription } from 'rxjs';

import { Component, Input, OnChanges, OnDestroy, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';

import { Language } from '../../../../shared/models/language';
import { Settings } from '../../../../shared/models/settings/settings';
import { SettingsService } from '../../../../shared/services/settings.service';
import { ToastService } from '../../../../shared/services/toast.service';

@Component({
  selector: 'app-settings-general',
  templateUrl: './settings-general.component.html',
  styleUrls: ['./settings-general.component.scss']
})
export class SettingsGeneralComponent implements OnInit, OnDestroy, OnChanges {
  @Input() settings: Settings;

  updateSub: Subscription;
  languages$: Observable<Language[]>;

  generalForm: FormGroup;
  nameControl = new FormControl('', [Validators.required]);
  languageControl = new FormControl('', [Validators.required]);
  exceptionLoggingControl = new FormControl(false);

  tvdbForm: FormGroup;
  tvdbKeyControl = new FormControl('');

  isSaving = false;

  constructor(
    private readonly settingsService: SettingsService,
    private readonly toastService: ToastService) {
    this.languages$ = this.settingsService.getLanguages();

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
    }
  }

  ngOnInit() {
  }

  saveGeneralForm() {
    if(this.checkForm(this.generalForm)) {
      this.isSaving = true;
      this.markFormAsUntouched(this.tvdbForm);

      this.settings.language = this.languageControl.value;
      this.settings.username = this.nameControl.value;
      this.settings.enableRollbarLogging = this.exceptionLoggingControl.value;
  
      this.saveSettings('SETTINGS.SAVED.GENERAL');
    }
  }

  saveTvdbForm() {
    if(this.checkForm(this.tvdbForm)) {
      this.isSaving = true;
      this.markFormAsUntouched(this.generalForm);

      this.settings.tvdb.apiKey = this.tvdbKeyControl.value;
      this.saveSettings('SETTINGS.SAVED.TVDB');
    }
  }

  private saveSettings(confirmMessage: string) {
    this.updateSub = this.settingsService.updateSettings(this.settings).subscribe((settings: Settings) => {
      this.toastService.showSuccess(confirmMessage);
    });

    this.updateSub.add(() => {
      this.isSaving = false;
    });
  }

  private checkForm(form: FormGroup): boolean {
    for (const i of Object.keys(form.controls)) {
      form.controls[i].markAsTouched();
      form.controls[i].updateValueAndValidity();
    }

    return form.valid;
  }

  private markFormAsUntouched(form: FormGroup) {
    for (const i of Object.keys(form.controls)) {
      form.controls[i].markAsUntouched();
    }
  }

  ngOnDestroy() {
    if (this.updateSub !== undefined) {
      this.updateSub.unsubscribe();
    }
  }
}
