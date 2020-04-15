import { Observable, Subscription } from 'rxjs';

import { Component, OnDestroy } from '@angular/core';

import { SettingsFacade } from '../../../shared/facades/settings.facade';
import { ConfigHelper } from '../../../shared/helpers/config-helper';
import { MediaServerPlugin } from '../../../shared/models/media-server/media-server-plugin';
import { Settings } from '../../../shared/models/settings/settings';
import { MediaServerService } from '../../../shared/services/media-server.service';

@Component({
  selector: 'es-plugin-overview',
  templateUrl: './plugin-overview.component.html',
  styleUrls: ['./plugin-overview.component.scss']
})
export class PluginOverviewComponent implements OnDestroy {
  plugins$: Observable<MediaServerPlugin[]>;

  settingsSub: Subscription;
  settings: Settings;

  constructor(
    private readonly embyService: MediaServerService,
    private readonly settingsFacade: SettingsFacade) {
    this.plugins$ = this.embyService.getPlugins();
    this.settingsSub = this.settingsFacade.getSettings().subscribe((settings: Settings) => {
      this.settings = settings;
    });
  }

  getFullAddress(): string {
    return ConfigHelper.getFullEmbyAddress(this.settings);
  }

  ngOnDestroy(): void {
    if (this.settingsSub !== undefined) {
      this.settingsSub.unsubscribe();
    }
  }
}
