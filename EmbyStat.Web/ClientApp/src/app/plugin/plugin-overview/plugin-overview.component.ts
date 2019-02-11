import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { PluginService } from '../service/plugin.service';
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
    private readonly pluginService: PluginService,
    private readonly settingsFacade: SettingsFacade) {
    this.plugins$ = pluginService.getPlugins();
    this.settings$ = settingsFacade.getSettings();
  }

  ngOnInit() {
  }

  getFullAddress(settings: Settings): string {
    return ConfigHelper.getFullEmbyAddress(settings);
  }
}
