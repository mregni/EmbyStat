import { Observable } from 'rxjs';

import { Component } from '@angular/core';

import { environment } from '../../../../environments/environment';
import { Settings } from '../../../shared/models/settings/settings';
import { SettingsService } from '../../../shared/services/settings.service';
import { About } from '../models/about';
import { AboutService } from '../services/about.service';

@Component({
  selector: 'es-about-overview',
  templateUrl: './about-overview.component.html',
  styleUrls: ['./about-overview.component.scss']
})
export class AboutOverviewComponent {
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
}
