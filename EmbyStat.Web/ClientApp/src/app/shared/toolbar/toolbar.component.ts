import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { Observable } from 'rxjs/Observable';

import { ConfigurationFacade } from '../../configuration/state/facade.configuration';
import { Configuration } from '../../configuration/models/configuration';
@Component({
  selector: 'app-toolbar',
  templateUrl: './toolbar.component.html',
  styleUrls: ['./toolbar.component.scss']
})
export class ToolbarComponent implements OnInit {

  public configuration$: Observable<Configuration>;

  @Output() toggleSideNav = new EventEmitter<void>();

  constructor(private configurationFacade: ConfigurationFacade) {
    this.configuration$ = configurationFacade.configuration$;
  }

  ngOnInit() {

  }
}
