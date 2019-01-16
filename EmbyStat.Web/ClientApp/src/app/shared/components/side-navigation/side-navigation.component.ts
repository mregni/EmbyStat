import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from "rxjs";

import { ConfigurationFacade } from '../../../configuration/state/facade.configuration';
import { Configuration } from '../../../configuration/models/configuration';

@Component({
  selector: 'app-side-navigation',
  templateUrl: './side-navigation.component.html',
  styleUrls: ['./side-navigation.component.scss']
})
export class SideNavigationComponent implements OnInit, OnDestroy {
  private configSub: Subscription;
  version: string;

  constructor(private configurationFacade: ConfigurationFacade) {
    this.configSub = this.configurationFacade.configuration$.subscribe((config: Configuration) => {
      this.version = config.version;
    });
  }

  ngOnInit() {
  }

  ngOnDestroy() {
    if (this.configSub !== undefined) {
      this.configSub.unsubscribe();
    }
  }

}
