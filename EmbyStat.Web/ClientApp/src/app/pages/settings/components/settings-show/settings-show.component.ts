import { Subscription } from 'rxjs';

import { Component, Input, OnDestroy, OnInit } from '@angular/core';

import { Settings } from '../../../../shared/models/settings/settings';
import { SettingsService } from '../../../../shared/services/settings.service';
import { ToastService } from '../../../../shared/services/toast.service';

@Component({
  selector: 'app-settings-show',
  templateUrl: './settings-show.component.html',
  styleUrls: ['./settings-show.component.scss']
})
export class SettingsShowComponent implements OnInit, OnDestroy {
  @Input() settings: Settings;
  updateSub: Subscription;

  newCollectionList: number[];

  isSaving = false;
  constructor(
    private readonly settingsService: SettingsService,
    private readonly toastService: ToastService) { }

  ngOnInit() {
  }

  saveFormCollectionTypes() {
    this.isSaving = true;
    this.settings.showCollectionTypes = this.newCollectionList;
    this.updateSub = this.settingsService.updateSettings(this.settings).subscribe((settings: Settings) => {
      this.toastService.showSuccess('SETTINGS.SAVED.SHOWS');
    });

    this.updateSub.add(() => {
      this.isSaving = false;
    });
  }

  onCollectionListChanged(event: number[]) {
    this.newCollectionList = event;
  }

  ngOnDestroy() {
    if (this.updateSub !== undefined) {
      this.updateSub.unsubscribe();
    }
  }
}
