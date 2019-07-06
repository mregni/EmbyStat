import { Observable, Subscription } from 'rxjs';

import { Component, OnDestroy, OnInit } from '@angular/core';

import { SettingsFacade } from '../../../shared/facades/settings.facade';
import { ConfigHelper } from '../../../shared/helpers/config-helper';
import { EmbyPlugin } from '../../../shared/models/emby/emby-plugin';
import { Settings } from '../../../shared/models/settings/settings';
import { EmbyService } from '../../../shared/services/emby.service';

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
    private readonly settingsFacade: SettingsFacade) {
    this.plugins$ = this.embyService.getPlugins();
    this.settingsSub = this.settingsFacade.getSettings().subscribe((settings: Settings) => {
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
