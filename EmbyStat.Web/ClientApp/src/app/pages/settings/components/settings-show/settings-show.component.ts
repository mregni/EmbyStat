import { Subscription } from 'rxjs';
import { SettingsFacade } from 'src/app/shared/facades/settings.facade';

import { Component, Input } from '@angular/core';

import { Settings } from '../../../../shared/models/settings/settings';
import { ToastService } from '../../../../shared/services/toast.service';

@Component({
  selector: 'es-settings-show',
  templateUrl: './settings-show.component.html',
  styleUrls: ['./settings-show.component.scss']
})
export class SettingsShowComponent {
  @Input() settings: Settings;

  newLibraryList: number[];

  isSaving = false;
  constructor(
    private readonly settingsFacade: SettingsFacade,
    private readonly toastService: ToastService) { }

  saveFormLibraryTypes(): void {
    this.isSaving = true;

    const settings = {...this.settings};
    settings.showLibraryTypes = this.newLibraryList;
    this.settingsFacade.updateSettings(settings);
    this.toastService.showSuccess('SETTINGS.SAVED.SHOWS');
    this.isSaving = false;

  }

  onLibraryListChanged(event: number[]): void {
    this.newLibraryList = event;
  }
}
