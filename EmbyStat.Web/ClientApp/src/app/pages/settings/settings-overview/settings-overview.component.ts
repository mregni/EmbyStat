import { Subscription } from 'rxjs';
import { SettingsFacade } from 'src/app/shared/facades/settings.facade';

import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

import { MediaServerTypeSelector } from '../../../shared/helpers/media-server-type-selector';
import { Settings } from '../../../shared/models/settings/settings';

@Component({
  selector: 'es-settings-overview',
  templateUrl: './settings-overview.component.html',
  styleUrls: ['./settings-overview.component.scss']
})
export class SettingsOverviewComponent implements OnInit, OnDestroy {
  private readonly paramSub: Subscription;

  settingsSub: Subscription;
  settings: Settings;
  selected = 0;
  serverTypeText: string;

  constructor(
    private readonly activatedRoute: ActivatedRoute,
    private readonly settingsFacade: SettingsFacade) {
    this.settingsSub = this.settingsFacade.getSettings().subscribe((settings: Settings) => {
      this.settings = settings;
      this.serverTypeText = MediaServerTypeSelector.getServerTypeString(this.settings.mediaServer.serverType);
    });

    this.paramSub = this.activatedRoute.params.subscribe(params => {
      const tab = params.tab;
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

    if (this.settingsSub !== undefined) {
      this.settingsSub.unsubscribe();
    }
  }
}
