import { Observable, Subscription } from 'rxjs';
import { SettingsFacade } from 'src/app/shared/facades/settings.facade';

import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

import { Settings } from '../../../shared/models/settings/settings';

@Component({
  selector: 'app-settings-overview',
  templateUrl: './settings-overview.component.html',
  styleUrls: ['./settings-overview.component.scss']
})
export class SettingsOverviewComponent implements OnInit, OnDestroy {
  private readonly paramSub: Subscription;

  settings$: Observable<Settings>;
  selected = 0;

  constructor(
    private readonly activatedRoute: ActivatedRoute,
    private readonly settingsFacade: SettingsFacade) {
    this.settings$ = this.settingsFacade.getSettings();
    this.paramSub = this.activatedRoute.params.subscribe(params => {
      const tab = params['tab'];
      switch (tab) {
      case 'emby':
        this.selected = 1;
        break;
      case 'movies':
        this.selected = 2;
        break;
      case 'shows':
        this.selected = 3;
        break;
      case 'updates':
        this.selected = 4;
        break;
      default:
        this.selected = 0;
      }
    });
  }

  ngOnInit() {
  }

  ngOnDestroy() {
    if (this.paramSub !== undefined) {
      this.paramSub.unsubscribe();
    }
  }
}
