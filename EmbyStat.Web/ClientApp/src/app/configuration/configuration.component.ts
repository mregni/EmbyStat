import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs/Observable';

import { ConfigurationFacade } from './state/facade.configuration';
import { Configuration } from './models/configuration';

@Component({
  selector: 'app-configuration',
  templateUrl: './configuration.component.html',
  styleUrls: ['./configuration.component.scss']
})
export class ConfigurationComponent implements OnInit {
  configuration$: Observable<Configuration>;

  constructor(private configurationFacade: ConfigurationFacade) { }

  ngOnInit() {
    
  }

}
