import { Observable, Subscription } from 'rxjs';

import { Component, OnDestroy, OnInit } from '@angular/core';

import { ConfigHelper } from '../../../shared/helpers/config-helper';
import { EmbyPlugin } from '../../../shared/models/emby/emby-plugin';
import { Settings } from '../../../shared/models/settings/settings';
import { EmbyService } from '../../../shared/services/emby.service';
import { SettingsService } from '../../../shared/services/settings.service';
import { TitleService } from '../../../shared/services/title.service';

@Component({
  selector: 'app-plugin-overview',
  templateUrl: './plugin-overview.component.html',
  styleUrls: ['./plugin-overview.component.scss']
})
export class PluginOverviewComponent implements OnInit, OnDestroy {
  plugins$: Observable<EmbyPlugin[]>;

  settingsSub: Subscription;
  settings: Settings;

  constructor(
    private readonly embyService: EmbyService,
    private readonly settingsService: SettingsService,
    private readonly titleService: TitleService) {
    this.titleService.updateTitle('Plugins');
    this.plugins$ = this.embyService.getPlugins();
    this.settingsSub = this.settingsService.getSettings().subscribe((settings: Settings) => {
      this.settings = settings;
    });
  }

  ngOnInit() {
  }

  getFullAddress(): string {
    return ConfigHelper.getFullEmbyAddress(this.settings);
  }

  ngOnDestroy() {
    if (this.settingsSub !== undefined) {
      this.settingsSub.unsubscribe();
    }
  }
}
