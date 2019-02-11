import { Component, Input, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs/Subscription';
import { DomSanitizer } from '@angular/platform-browser';
import { PersonPoster } from '../../models/person-poster';
import { SettingsFacade } from '../../../settings/state/facade.settings';
import {Settings } from '../../../settings/models/settings';
import { ConfigHelper } from '../../helpers/configHelper';

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

  getBackground() {
    if (this.settings === undefined) {
      return '';
    }

    if (this.poster.tag === null) {
      return this._sanitizer.bypassSecurityTrustStyle('url(../../../../assets/images/backgrounds/defaultPerson.png)');
    }

    const fullAddress = ConfigHelper.getFullEmbyAddress(this.settings);
    return this._sanitizer.bypassSecurityTrustStyle(`url(${fullAddress}/emby/Items/${this.poster.mediaId}/Images/Primary?maxHeight=350&tag=${this.poster.tag}&quality=90)`);
  }

  openPerson(): void {
    window.open(`${ConfigHelper.getFullEmbyAddress(this.settings)}/web/index.html#!/itemdetails.html?id=${this.poster.mediaId}`, '_blank');
  }

  needsBarAndTranslation(title: string): boolean {
    return title.length > 0 && title.startsWith('MOVIES');
  }

  needsBarButNoTranslation(title: string): boolean {
    return title.length > 0 && !title.startsWith('MOVIES');
  }

  ngOnDestroy(): void {
    if (this.settingsSub !== undefined) {
      this.settingsSub.unsubscribe();
    }
  }
}
