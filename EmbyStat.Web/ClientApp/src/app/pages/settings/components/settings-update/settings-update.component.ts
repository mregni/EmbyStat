import { Observable, of, Subscription } from 'rxjs';
import { catchError, map } from 'rxjs/operators';

import { Component, Input, OnChanges, OnDestroy, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';

import { SettingsFacade } from '../../../../shared/facades/settings.facade';
import { Settings } from '../../../../shared/models/settings/settings';
import { UpdateResult } from '../../../../shared/models/settings/update-result';
import { SystemService } from '../../../../shared/services/system.service';
import { ToastService } from '../../../../shared/services/toast.service';

@Component({
  selector: 'es-settings-update',
  templateUrl: './settings-update.component.html',
  styleUrls: ['./settings-update.component.scss']
})
export class SettingsUpdateComponent implements OnInit, OnDestroy, OnChanges {
  @Input() settings: Settings;
  updatingSub: Subscription;

  updateResult$: Observable<UpdateResult>;
  onMaster = true;

  form: FormGroup;
  autoUpdateControl = new FormControl('', [Validators.required]);
  trainControl = new FormControl({value: '', disabled: !this.onMaster}, [Validators.required]);
  updateCheckFailed = false;

  trainOptions = [];
  isSaving = false;

  constructor(
    private readonly settingsFacade: SettingsFacade,
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
      if (this.settings.updateTrain !== 2){
        this.trainControl.disable();
      }
    }
  }

  save() {
    this.isSaving = true;
    this.form.disable();

    const settings = {...this.settings};
    settings.updateTrain = this.trainControl.value;
    settings.autoUpdate = this.autoUpdateControl.value;
    this.settingsFacade.updateSettings(settings);
    this.toastService.showSuccess('SETTINGS.SAVED.UPDATES');
    this.isSaving = false;
    this.form.enable();
  }

  private setUpdateState(state: boolean) {
    const settings = {...this.settings};
    settings.updateInProgress = state;
    this.settingsFacade.updateSettings(settings);
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
    if (this.updatingSub !== undefined) {
      this.updatingSub.unsubscribe();
    }
  }
}
