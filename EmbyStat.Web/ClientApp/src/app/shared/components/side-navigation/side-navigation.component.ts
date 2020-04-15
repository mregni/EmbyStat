import { Subscription } from 'rxjs';

import { Component, OnDestroy } from '@angular/core';

import { SettingsFacade } from '../../facades/settings.facade';
import { Settings } from '../../models/settings/settings';

@Component({
  selector: 'es-side-navigation',
  templateUrl: './side-navigation.component.html',
  styleUrls: ['./side-navigation.component.scss']
})
export class SideNavigationComponent implements OnDestroy {
  private configSub: Subscription;
  version: string;

  constructor(private settingsFacade: SettingsFacade) {
    this.configSub = this.settingsFacade.getSettings().subscribe((settings: Settings) => {
      this.version = settings.version;
    });
  }

  ngOnDestroy(): void {
    if (this.configSub !== undefined) {
      this.configSub.unsubscribe();
    }
  }
}
