import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs/Observable';

import { ConfigurationFacade } from './state/configuration.facade';
import { Configuration } from './models/configuration';

@Component({
  selector: 'app-configuration',
  templateUrl: './configuration.component.html',
  styleUrls: ['./configuration.component.scss']
})
export class ConfigurationComponent implements OnInit {

  constructor(private configurationFacade: ConfigurationFacade) { }

  configuration$: Observable<Configuration>;

  ngOnInit() {
    
  }

}
