import { Subscription } from 'rxjs';

import { Component, Input, OnInit } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';

import { ConfigHelper } from '../../../helpers/config-helper';
import { MoviePoster } from '../../../models/movie/movie-poster';
import { Settings } from '../../../models/settings/settings';
import { SettingsService } from '../../../services/settings.service';

@Component({
  selector: 'app-movie-poster',
  templateUrl: './movie-poster.component.html',
  styleUrls: ['./movie-poster.component.scss']
})
export class MoviePosterComponent implements OnInit {
  settingsSub: Subscription;
  settings: Settings;
  @Input() poster: MoviePoster;

  constructor(
    private readonly settingsService: SettingsService,
    private readonly sanitizer: DomSanitizer) {
    this.settingsSub = this.settingsService.getSettings().subscribe(data => this.settings = data);
  }

  ngOnInit() {
  }

  getPoster() {
    if (this.settings === undefined) {
      return '';
    }
    const fullAddress = ConfigHelper.getFullEmbyAddress(this.settings);
    const url = `url(${fullAddress}/emby/Items/${this.poster.mediaId}/Images/Primary?maxHeight=350&tag=${this.poster.tag}&quality=90&enableimageenhancers=false)`;
    return this.sanitizer.bypassSecurityTrustStyle(url);
  }

  openMovie() {
    window.open(`${ConfigHelper.getFullEmbyAddress(this.settings)}/web/index.html#!/itemdetails.html?id=${this.poster.mediaId}`, '_blank');
  }
}
