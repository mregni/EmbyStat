import { Subscription } from 'rxjs';

import { Component, Input, OnChanges, OnDestroy, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';

import { Settings } from '../../../../shared/models/settings/settings';
import { SettingsService } from '../../../../shared/services/settings.service';
import { ToastService } from '../../../../shared/services/toast.service';

@Component({
  selector: 'app-settings-movie',
  templateUrl: './settings-movie.component.html',
  styleUrls: ['./settings-movie.component.scss']
})
export class SettingsMovieComponent implements OnInit, OnDestroy, OnChanges {
  @Input() settings: Settings;
  updateSub: Subscription;

  formToShort: FormGroup;
  toShortMovieControl = new FormControl('', [Validators.required]);

  newCollectionList: number[];
  isSaving = false;

  constructor(
    private readonly settingsService: SettingsService,
    private readonly toastService: ToastService) {
    this.formToShort = new FormGroup({
      toShortMovie: this.toShortMovieControl
    });
  }

  ngOnInit() {
  }

  ngOnChanges(): void {
    console.log(this.settings);
    if (this.settings !== undefined) {
      this.toShortMovieControl.setValue(this.settings.toShortMovie);
    }
  }

  saveToShortForm() {
    if (this.checkForm(this.formToShort)) {
      this.isSaving = true;
      this.settings.toShortMovie = this.toShortMovieControl.value;
      this.updateSub = this.settingsService.updateSettings(this.settings).subscribe((settings: Settings) => {
        this.toastService.showSuccess('SETTINGS.SAVED.MOVIES');
      });

      this.updateSub.add(() => {
        this.isSaving = false;
      });
    }
  }

  saveCollectionTypesForm() {
    this.isSaving = true;
    this.settings.movieCollectionTypes = this.newCollectionList;
    this.updateSub = this.settingsService.updateSettings(this.settings).subscribe((settings: Settings) => {
      this.toastService.showSuccess('SETTINGS.SAVED.MOVIES');
    });

    this.updateSub.add(() => {
      this.isSaving = false;
    });
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
    if (this.updateSub !== undefined) {
      this.updateSub.unsubscribe();
    }
  }
}
