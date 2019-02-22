import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from "rxjs";

import { SettingsFacade } from '../../../settings/state/facade.settings';
import {Settings } from '../../../settings/models/settings';

@Component({
  selector: 'app-side-navigation',
  templateUrl: './side-navigation.component.html',
  styleUrls: ['./side-navigation.component.scss']
})
export class SideNavigationComponent implements OnInit, OnDestroy {
  private configSub: Subscription;
  version: string;

  constructor(private settingsFacade: SettingsFacade) {
    this.configSub = this.settingsFacade.settings$.subscribe((settings: Settings) => {
      this.version = settings.version;
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
