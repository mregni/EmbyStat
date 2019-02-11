import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { EmbyService } from '../../shared/services/emby.service';
import { SettingsFacade } from '../../settings/state/facade.settings';
import { EmbyPlugin } from '../../shared/models/emby/emby-plugin';
import { Settings } from '../../settings/models/settings';
import { ConfigHelper } from '../../shared/helpers/configHelper';

@Component({
  selector: 'app-plugin-overview',
  templateUrl: './plugin-overview.component.html',
  styleUrls: ['./plugin-overview.component.scss']
})
export class PluginOverviewComponent implements OnInit {
  plugins$: Observable<EmbyPlugin[]>;
  settings$: Observable<Settings>;

  constructor(
    private readonly embyService: EmbyService,
    private readonly settingsFacade: SettingsFacade) {
    this.plugins$ = embyService.getPlugins();
    this.settings$ = settingsFacade.getSettings();
  }

  ngOnInit() {
  }

  getFullAddress(settings: Settings): string {
    return ConfigHelper.getFullEmbyAddress(settings);
  }
}
