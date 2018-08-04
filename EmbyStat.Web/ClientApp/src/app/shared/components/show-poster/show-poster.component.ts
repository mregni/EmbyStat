import { Component, Input, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs/Subscription';
import { DomSanitizer } from '@angular/platform-browser';
import { ConfigurationFacade } from '../../../configuration/state/facade.configuration';
import { Configuration } from '../../../configuration/models/configuration';
import { ShowPoster } from '../../models/showPoster';
import { ConfigHelper } from '../../helpers/configHelper';

@Component({
  selector: 'app-show-poster',
  templateUrl: './show-poster.component.html',
  styleUrls: ['./show-poster.component.scss']
})
export class ShowPosterComponent implements OnDestroy {
  configurationSub: Subscription;
  configuration: Configuration;
  @Input() poster: ShowPoster;

  constructor(private configurationFacade: ConfigurationFacade, private _sanitizer: DomSanitizer) {
    this.configurationSub = configurationFacade.getConfiguration().subscribe(data => this.configuration = data);
  }

  getBackground() {
    if (this.configuration === undefined) {
      return '';
    }
    return this._sanitizer.bypassSecurityTrustStyle(`url(${ConfigHelper.getFullEmbyAddress(this.configuration)}/emby/Items/${this.poster.mediaId}/Images/Primary?maxHeight=350&tag=${this.poster.tag}&quality=90)`);
  }

  openShow(): void {
    window.open(`${ConfigHelper.getFullEmbyAddress(this.configuration)}/web/index.html#!/itemdetails.html?id=${this.poster.mediaId}`, '_blank');
  }

  ngOnDestroy(): void {
    if (this.configurationSub !== undefined) {
      this.configurationSub.unsubscribe();
    }
  }
}
