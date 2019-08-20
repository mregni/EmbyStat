import { Subscription } from 'rxjs';

import { Component, Input, OnChanges, OnDestroy, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';

import { SettingsFacade } from '../../../../shared/facades/settings.facade';
import { Settings } from '../../../../shared/models/settings/settings';
import { ToastService } from '../../../../shared/services/toast.service';

@Component({
  selector: 'app-settings-movie',
  templateUrl: './settings-movie.component.html',
  styleUrls: ['./settings-movie.component.scss']
})
export class SettingsMovieComponent implements OnInit, OnDestroy, OnChanges {
  @Input() settings: Settings;

  formToShort: FormGroup;
  toShortMovieControl = new FormControl('', [Validators.required]);
  toShortMovieEnabledControl = new FormControl('', [Validators.required]);

  newCollectionList: number[];
  isSaving = false;

  constructor(
    private readonly settingsFacade: SettingsFacade,
    private readonly toastService: ToastService) {
    this.formToShort = new FormGroup({
      toShortMovie: this.toShortMovieControl,
      toShortMovieEnabled: this.toShortMovieEnabledControl
    });
  }

  ngOnInit() {
  }

  ngOnChanges(): void {
    if (this.settings !== undefined) {
      console.log(this.settings);
      this.toShortMovieControl.setValue(this.settings.toShortMovie);
      this.toShortMovieEnabledControl.setValue(this.settings.toShortMovieEnabled);
    }
  }

  saveToShortForm() {
    if (this.checkForm(this.formToShort)) {
      this.isSaving = true;

      const settings = { ...this.settings };
      settings.toShortMovie = this.toShortMovieControl.value;
      settings.toShortMovieEnabled = this.toShortMovieEnabledControl.value;
      this.settingsFacade.updateSettings(settings);
      this.toastService.showSuccess('SETTINGS.SAVED.MOVIES');
      this.isSaving = false;
    }
  }

  saveCollectionTypesForm() {
    this.isSaving = true;

    const settings = { ...this.settings };
    settings.movieLibraryTypes = this.newCollectionList;
    this.settingsFacade.updateSettings(settings);
    this.toastService.showSuccess('SETTINGS.SAVED.MOVIES');
    this.isSaving = false;
  }

  onCollectionListChanged(event: number[]) {
    this.newCollectionList = event;
  }

  private checkForm(form: FormGroup): boolean {
    for (const i of Object.keys(form.controls)) {
      form.controls[i].markAsTouched();
      form.controls[i].updateValueAndValidity();
    }

    return form.valid;
  }

  ngOnDestroy() {

  }
}
