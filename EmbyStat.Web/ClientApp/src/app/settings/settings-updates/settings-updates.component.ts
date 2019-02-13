import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { Subscription ,  Observable } from 'rxjs';

import { SettingsFacade } from '../state/facade.settings';
import { Settings } from '../models/settings';
import { UpdateResult } from '../../shared/models/update-result';
import { ToastService } from '../../shared/services/toast.service';
import { SystemService } from '../../shared/services/system.service';

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
  autoUpdateControl = new FormControl('', [Validators.required] );
  trainControl = new FormControl('', [Validators.required]);

  constructor(
    private readonly settingsFacade: SettingsFacade,
    private readonly toaster: ToastService,
    private readonly systemService: SystemService) {
    this.settings$ = this.settingsFacade.getSettings();

    this.updateResult$ = this.systemService.checkForUpdate();

    this.form = new FormGroup({
      autoUpdate: this.autoUpdateControl,
      train: this.trainControl
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
