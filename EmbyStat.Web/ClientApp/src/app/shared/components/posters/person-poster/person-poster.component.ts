import { Subscription } from 'rxjs';
import { SettingsFacade } from 'src/app/shared/facades/settings.facade';

import { Component, Input, OnDestroy } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';

import { ConfigHelper } from '../../../helpers/config-helper';
import { PersonPoster } from '../../../models/common/person-poster';
import { Settings } from '../../../models/settings/settings';

@Component({
  selector: 'app-person-poster',
  templateUrl: './person-poster.component.html',
  styleUrls: ['./person-poster.component.scss']
})
export class PersonPosterComponent implements OnDestroy {
  settingsSub: Subscription;
  settings: Settings;
  @Input() poster: PersonPoster;

  constructor(private settingsFacade: SettingsFacade, private _sanitizer: DomSanitizer) {
    this.settingsSub = settingsFacade.getSettings().subscribe(data => this.settings = data);
  }

  getPoster() {
    if (this.settings === undefined) {
      return '';
    }

    if (this.poster.tag === null) {
      return this._sanitizer.bypassSecurityTrustStyle('url(../../../../assets/images/backgrounds/defaultPerson.png)');
    }

    const fullAddress = ConfigHelper.getFullEmbyAddress(this.settings);
    const url = `url(${fullAddress}/emby/Items/${this.poster.mediaId}/Images/Primary?maxHeight=350&tag=${this.poster.tag}&quality=90&enableimageenhancers=false)`;
    return this._sanitizer.bypassSecurityTrustStyle(url);
  }

  openPerson(): void {
    window.open(`${ConfigHelper.getFullEmbyAddress(this.settings)}/web/index.html#!/itemdetails.html?id=${this.poster.mediaId}`, '_blank');
  }

  needsBarAndTranslation(title: string): boolean {
    if (title !== null && title.length !== undefined) {
      return title.length > 0 && title.startsWith('COMMON');
    }

    return false;
  }

  needsBarButNoTranslation(title: string): boolean {
    if (title != null && title.length !== undefined) {
      return title.length > 0 && !title.startsWith('COMMON');
    }

    return false;
  }

  ngOnDestroy(): void {
    if (this.settingsSub !== undefined) {
      this.settingsSub.unsubscribe();
    }
  }
}
