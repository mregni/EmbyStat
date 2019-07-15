import { Subscription } from 'rxjs';
import { SettingsFacade } from 'src/app/shared/facades/settings.facade';

import { Component, Input, OnDestroy, OnInit } from '@angular/core';

import { Settings } from '../../../../shared/models/settings/settings';
import { ToastService } from '../../../../shared/services/toast.service';

@Component({
  selector: 'app-settings-show',
  templateUrl: './settings-show.component.html',
  styleUrls: ['./settings-show.component.scss']
})
export class SettingsShowComponent implements OnInit, OnDestroy {
  @Input() settings: Settings;

  newCollectionList: number[];

  isSaving = false;
  constructor(
    private readonly settingsFacade: SettingsFacade,
    private readonly toastService: ToastService) { }

  ngOnInit() {
  }

  saveFormCollectionTypes() {
    this.isSaving = true;

    const settings = {...this.settings};
    settings.showCollectionTypes = this.newCollectionList;
    this.settingsFacade.updateSettings(settings);
    this.toastService.showSuccess('SETTINGS.SAVED.SHOWS');
    this.isSaving = false;

  }

  onCollectionListChanged(event: number[]) {
    this.newCollectionList = event;
  }

  ngOnDestroy() {

  }
}
