import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { PluginService } from '../service/plugin.service';
import { ConfigurationFacade } from '../../configuration/state/facade.configuration';
import { EmbyPlugin } from '../../shared/models/emby/emby-plugin';
import { Configuration } from '../../configuration/models/configuration';

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

}
