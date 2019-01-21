import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { PluginService } from '../service/plugin.service';
import { ConfigurationFacade } from '../../configuration/state/facade.configuration';
import { EmbyPlugin } from '../../shared/models/emby/emby-plugin';
import { Configuration } from '../../configuration/models/configuration';
import { ConfigHelper } from '../../shared/helpers/configHelper';

@Component({
  selector: 'app-plugin-overview',
  templateUrl: './plugin-overview.component.html',
  styleUrls: ['./plugin-overview.component.scss']
})
export class PluginOverviewComponent implements OnInit {
  plugins$: Observable<EmbyPlugin[]>;
  configuration$: Observable<Configuration>;

  constructor(private pluginService: PluginService, private configurationFacade: ConfigurationFacade) {
    this.plugins$ = pluginService.getPlugins();
    this.configuration$ = configurationFacade.getConfiguration();
  }

  ngOnInit() {
  }

  getFullAddress(config: Configuration): string {
    return ConfigHelper.getFullEmbyAddress(config);
  }
}
