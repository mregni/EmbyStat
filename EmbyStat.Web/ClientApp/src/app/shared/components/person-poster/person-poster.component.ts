import { Component, Input, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs/Subscription';
import { DomSanitizer } from '@angular/platform-browser';
import { PersonPoster } from '../../models/person-poster';
import { ConfigurationFacade } from '../../../configuration/state/facade.configuration';
import { Configuration } from '../../../configuration/models/configuration';
import { ConfigHelper } from '../../helpers/configHelper';

@Component({
  selector: 'app-person-poster',
  templateUrl: './person-poster.component.html',
  styleUrls: ['./person-poster.component.scss']
})
export class PersonPosterComponent implements OnDestroy {
  configurationSub: Subscription;
  configuration: Configuration;
  @Input() poster: PersonPoster;

  constructor(private configurationFacade: ConfigurationFacade, private _sanitizer: DomSanitizer) {
    this.configurationSub = configurationFacade.getConfiguration().subscribe(data => this.configuration = data);
  }

  getBackground() {
    if (this.configuration === undefined) {
      return '';
    }

    if (this.poster.tag === null) {
      return this._sanitizer.bypassSecurityTrustStyle('url(../../../../assets/images/backgrounds/defaultPerson.png)');
    }

    const fullAddress = ConfigHelper.getFullEmbyAddress(this.configuration);
    return this._sanitizer.bypassSecurityTrustStyle(`url(${fullAddress}/emby/Items/${this.poster.mediaId}/Images/Primary?maxHeight=350&tag=${this.poster.tag}&quality=90)`);
  }

  openPerson(): void {
    window.open(`${ConfigHelper.getFullEmbyAddress(this.configuration)}/web/index.html#!/itemdetails.html?id=${this.poster.mediaId}`, '_blank');
  }

  needsBarAndTranslation(title: string): boolean {
    return title.length > 0 && title.startsWith('MOVIES');
  }

  needsBarButNoTranslation(title: string): boolean {
    return title.length > 0 && !title.startsWith('MOVIES');
  }

  ngOnDestroy(): void {
    if (this.configurationSub !== undefined) {
      this.configurationSub.unsubscribe();
    }
  }
}
