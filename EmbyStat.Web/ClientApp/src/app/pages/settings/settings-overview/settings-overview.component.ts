import { Observable, Subscription } from 'rxjs';

import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

import { Settings } from '../../../shared/models/settings/settings';
import { SettingsService } from '../../../shared/services/settings.service';

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
    private readonly settingsService: SettingsService) {
    this.settings$ = this.settingsService.getSettings();
    this.paramSub = this.activatedRoute.params.subscribe(params => {
      const tab = params['tab'];
      switch (tab) {
      case "emby":
        this.selected = 1;
        break;
      case "movies":
        this.selected = 2;
        break;
      case "shows":
        this.selected = 3;
        break;
      case "updates":
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
