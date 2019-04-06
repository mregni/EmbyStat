import { Component, Input, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { DomSanitizer } from '@angular/platform-browser';
import { MoviePoster } from '../../models/movie-poster';
import { SettingsFacade } from '../../../settings/state/facade.settings';
import { Settings } from '../../../settings/models/settings';
import { ConfigHelper } from '../../helpers/configHelper';

@Component({
  selector: 'app-movie-poster',
  templateUrl: './movie-poster.component.html',
  styleUrls: ['./movie-poster.component.scss']
})
export class MoviePosterComponent implements OnDestroy {
  settingsSub: Subscription;
  settings: Settings;
  @Input() poster: MoviePoster;

  constructor(
    private readonly settingsFacade: SettingsFacade,
    private readonly sanitizer: DomSanitizer) {
    this.settingsSub = settingsFacade.getSettings().subscribe(data => this.settings = data);
  }

  getBackground() {
    if (this.settings === undefined) {
      return '';
    }
    const fullAddress = ConfigHelper.getFullEmbyAddress(this.settings);
    return this.sanitizer.bypassSecurityTrustStyle(`url(${fullAddress}/emby/Items/${this.poster.mediaId}/Images/Primary?maxHeight=350&tag=${this.poster.tag}&quality=90&enableimageenhancers=false)`);
  }

  openMovie(): void {
    window.open(`${ConfigHelper.getFullEmbyAddress(this.settings)}/web/index.html#!/itemdetails.html?id=${this.poster.mediaId}`, '_blank');
  }

  ngOnDestroy(): void {
    if (this.settingsSub !== undefined) {
      this.settingsSub.unsubscribe();
    }
  }
}
