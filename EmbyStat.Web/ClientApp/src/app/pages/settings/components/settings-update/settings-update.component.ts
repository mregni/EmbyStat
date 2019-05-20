import { Observable, of, Subscription } from 'rxjs';
import { catchError, map } from 'rxjs/operators';

import { Component, Input, OnChanges, OnDestroy, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';

import { Settings } from '../../../../shared/models/settings/settings';
import { UpdateResult } from '../../../../shared/models/settings/update-result';
import { SettingsService } from '../../../../shared/services/settings.service';
import { SystemService } from '../../../../shared/services/system.service';
import { ToastService } from '../../../../shared/services/toast.service';

@Component({
  selector: 'app-settings-update',
  templateUrl: './settings-update.component.html',
  styleUrls: ['./settings-update.component.scss']
})
export class SettingsUpdateComponent implements OnInit, OnDestroy, OnChanges {
  @Input() settings: Settings;
  updateSub: Subscription;
  updatingSub: Subscription;

  updateResult$: Observable<UpdateResult>;
  onMaster = true;

  form: FormGroup;
  autoUpdateControl = new FormControl('', [Validators.required]);
  trainControl = new FormControl('', [Validators.required]);
  updateCheckFailed = false;

  trainOptions = [];
  isSaving = false;

  constructor(
    private readonly settingsService: SettingsService,
    private readonly toastService: ToastService,
    private readonly systemService: SystemService,
    private readonly translateService: TranslateService
  ) {
    this.checkUpdate();

    this.form = new FormGroup({
      autoUpdate: this.autoUpdateControl,
      train: this.trainControl
    });

    this.translateService.get(['SETTINGS.UPDATE.MASTER', 'SETTINGS.UPDATE.BETA']).subscribe((value) => {
      this.trainOptions.push({ key: value['SETTINGS.UPDATE.MASTER'], value: 2 });
      this.trainOptions.push({ key: value['SETTINGS.UPDATE.BETA'], value: 1 });
    });
  }

  ngOnInit() {
  }

  ngOnChanges(): void {
    if (this.settings !== undefined) {
      this.autoUpdateControl.setValue(this.settings.autoUpdate);
      this.trainControl.setValue(this.settings.updateTrain);
      this.onMaster = this.settings.updateTrain === 2;
    }
  }

  save() {
    this.isSaving = true;
    this.settings.updateTrain = this.trainControl.value;
    this.settings.autoUpdate = this.autoUpdateControl.value;
    console.log(this.settings);
    this.updateSub = this.settingsService.updateSettings(this.settings).subscribe((settings: Settings) => {
      this.toastService.showSuccess('SETTINGS.SAVED.UPDATES');
    });

    this.updateSub.add(() => {
      this.isSaving = false;
    });
  }

  private setUpdateState(state: boolean) {
    this.settings.updateInProgress = state;
    this.updateSub = this.settingsService.updateSettings(this.settings).subscribe((settings: Settings) => {

    });
  }

  checkUpdate() {
    this.updateResult$ = this.systemService.checkForUpdate()
      .pipe(catchError((err, caught) => {
        this.updateCheckFailed = true;
        this.setUpdateState(false);
        return of(err);
      }) as any);
  }

  startUpdate() {
    this.setUpdateState(true);
    this.systemService.startUpdate().pipe(catchError((err, caught) => {
      this.updateCheckFailed = true;
      this.setUpdateState(false);
      return of(err);
    }) as any)
      .subscribe((newVersion: boolean) => {
        if (!newVersion) {
          this.setUpdateState(false);
          this.updateCheckFailed = false;
        }
      });
  }

  ngOnDestroy() {
    if (this.updateSub !== undefined) {
      this.updateSub.unsubscribe();
    }

    if (this.updatingSub !== undefined) {
      this.updatingSub.unsubscribe();
    }
  }
}
