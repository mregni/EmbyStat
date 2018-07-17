import { Component, Input, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs/Subscription';
import { DomSanitizer } from '@angular/platform-browser';
import { MoviePoster } from '../../models/moviePoster';
import { ConfigurationFacade } from '../../../configuration/state/facade.configuration';
import { Configuration } from '../../../configuration/models/configuration';

@Component({
  selector: 'app-movie-poster',
  templateUrl: './movie-poster.component.html',
  styleUrls: ['./movie-poster.component.scss']
})
export class MoviePosterComponent implements OnDestroy {
  configurationSub: Subscription;
  configuration: Configuration;
  @Input() poster: MoviePoster;

  constructor(private configurationFacade: ConfigurationFacade, private _sanitizer: DomSanitizer) {
    this.configurationSub = configurationFacade.getConfiguration().subscribe(data => this.configuration = data);
  }

  getBackground() {
    if (this.configuration === undefined) {
      return '';
    }
    return this._sanitizer.bypassSecurityTrustStyle(`url(${this.configuration.embyServerAddress}/emby/Items/${this.poster.mediaId}/Images/Primary?maxHeight=350&tag=${this.poster.tag}&quality=90)`);
  }

  openMovie(): void {
    window.open(`${this.configuration.embyServerAddress}/web/index.html#!/itemdetails.html?id=${this.poster.mediaId}`, '_blank');
  }

  ngOnDestroy(): void {
    if (this.configurationSub !== undefined) {
      this.configurationSub.unsubscribe();
    }
  }
}
