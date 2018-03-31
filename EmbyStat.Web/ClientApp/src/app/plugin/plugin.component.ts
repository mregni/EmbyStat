import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { PluginFacade } from './state/facade.plugin';
import { ConfigurationFacade } from '../configuration/state/facade.configuration';
import { EmbyPlugin } from './models/embyPlugin';
import { Configuration } from '../configuration/models/configuration';

@Component({
  selector: 'app-plugin',
  templateUrl: './plugin.component.html',
  styleUrls: ['./plugin.component.scss'],
  
})
export class PluginComponent implements OnInit {
  plugins$: Observable<EmbyPlugin[]>;
  configuration$: Observable<Configuration>;

  constructor(private pluginFacade: PluginFacade, private configurationFacade: ConfigurationFacade) {
    this.plugins$ = pluginFacade.getPlugins();
    this.configuration$ = configurationFacade.getConfiguration();
  }

  ngOnInit() {
  }

}
