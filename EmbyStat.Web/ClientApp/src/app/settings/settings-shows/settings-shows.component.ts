import { Component, OnInit, OnDestroy } from '@angular/core';
import { Observable ,  Subscription } from 'rxjs';

import { SettingsFacade } from '../state/facade.settings';
import { Settings } from '../models/settings';
import { ToastService } from '../../shared/services/toast.service';

@Component({
  selector: 'app-settings-shows',
  templateUrl: './settings-shows.component.html',
  styleUrls: ['./settings-shows.component.scss']
})
export class SettingsShowsComponent implements OnInit, OnDestroy {
  settings$: Observable<Settings>;
  private settings: Settings;

  settingsChangedSub: Subscription;
  newCollectionList: number[];

  constructor(
    private readonly settingsFacade: SettingsFacade,
    private readonly toaster: ToastService) {
    this.settings$ = this.settingsFacade.getSettings();

    this.settingsChangedSub = this.settings$.subscribe((settings: Settings) => {
      this.settings = settings;
    });
  }

  public saveFormCollectionTypes() {
    const settings = { ...this.settings };
    settings.showCollectionTypes = this.newCollectionList;
    this.settingsFacade.updateSettings(settings);
    this.toaster.pushSuccess('SETTINGS.SAVED.SHOWS');
  }

  public onCollectionListChanged(event: number[]) {
    this.newCollectionList = event;
  }

  ngOnInit() {
  }

  ngOnDestroy() {
    if (this.settingsChangedSub !== undefined) {
      this.settingsChangedSub.unsubscribe();
    }
  }
}
