import { Observable } from 'rxjs';

import { Component, OnInit } from '@angular/core';

import { environment } from '../../../../environments/environment';
import { About } from '../models/about';
import { AboutService } from '../services/about.service';
import { SettingsService } from '../../../shared/services/settings.service';
import { Settings } from '../../../shared/models/settings/settings';

@Component({
  selector: 'app-about-overview',
  templateUrl: './about-overview.component.html',
  styleUrls: ['./about-overview.component.scss']
})
export class AboutOverviewComponent implements OnInit {
  about$: Observable<About>;
  settings$: Observable<Settings>;
  environment;

  constructor(
    private readonly aboutService: AboutService,
    private readonly settingsService: SettingsService) {
    this.about$ = this.aboutService.getAbout();
    this.settings$ = this.settingsService.getSettings();
    this.environment = environment;
  }

  ngOnInit() {
  }

}
