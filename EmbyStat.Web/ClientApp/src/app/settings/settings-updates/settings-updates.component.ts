import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { Subscription, Observable, of } from 'rxjs';
import { catchError, map } from 'rxjs/operators';

import { SettingsFacade } from '../state/facade.settings';
import { Settings } from '../models/settings';
import { UpdateResult } from '../../shared/models/update-result';
import { ToastService } from '../../shared/services/toast.service';
import { SystemService } from '../../shared/services/system.service';
import { TranslateService } from '@ngx-translate/core';
@Component({
  selector: 'app-settings-updates',
  templateUrl: './settings-updates.component.html',
  styleUrls: ['./settings-updates.component.scss']
})
export class SettingsUpdatesComponent implements OnInit, OnDestroy {
  settings$: Observable<Settings>;
  updateResult$: Observable<UpdateResult>;
  settings: Settings;
  settingsChangedSub: Subscription;
  updatingSub: Subscription;
  onMaster = true;

  form: FormGroup;
  autoUpdateControl = new FormControl('', [Validators.required]);
  trainControl = new FormControl('', [Validators.required]);
  updateCheckFailed = false;

  trainOptions = [];

  constructor(
    private readonly settingsFacade: SettingsFacade,
    private readonly toaster: ToastService,
    private readonly systemService: SystemService,
    private readonly translateService: TranslateService) {
    this.settings$ = this.settingsFacade.getSettings();

    this.updateResult$ = this.systemService.checkForUpdate()
      .pipe(catchError((err, caught) => {
        this.updateCheckFailed = true;
        this.setUpdateState(false);
        return of(err);
      }) as any);

    this.form = new FormGroup({
      autoUpdate: this.autoUpdateControl,
      train: this.trainControl
    });

    this.translateService.get(['SETTINGS.UPDATE.MASTER', 'SETTINGS.UPDATE.BETA']).subscribe((value) => {

      this.trainOptions.push({ key: value['SETTINGS.UPDATE.MASTER'], value: 2 });
      this.trainOptions.push({ key: value['SETTINGS.UPDATE.BETA'], value: 1 });
    });

    this.settingsChangedSub = this.settings$.subscribe((settings: Settings) => {
      this.settings = settings;
      this.autoUpdateControl.setValue(settings.autoUpdate);
      this.trainControl.setValue(settings.updateTrain);
      this.onMaster = settings.updateTrain === 2;
    });
  }

  save() {
    const settings = { ...this.settings };
    settings.updateTrain = this.trainControl.value;
    settings.autoUpdate = this.autoUpdateControl.value;
    this.settingsFacade.updateSettings(settings);
    this.toaster.pushSuccess('SETTINGS.SAVED.UPDATES');
  }

  ngOnInit() {
  }

  public startUpdate() {
    this.setUpdateState(true);
    this.systemService.checkAndStartUpdate().subscribe((newVersion: boolean) => {
      if (!newVersion) {
        this.setUpdateState(false);
        this.updateCheckFailed = false;
      }
    });
  }

  private setUpdateState(state: boolean) {
    const settings = { ...this.settings };
    settings.updateInProgress = state;
    this.settingsFacade.updateSettings(settings);
  }

  ngOnDestroy() {
    if (this.settingsChangedSub !== undefined) {
      this.settingsChangedSub.unsubscribe();
    }

    if (this.updatingSub !== undefined) {
      this.updatingSub.unsubscribe();
    }
  }
}
