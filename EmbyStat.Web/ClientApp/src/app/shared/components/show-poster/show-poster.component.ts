import { Component, Input, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { DomSanitizer } from '@angular/platform-browser';
import { SettingsFacade } from '../../../settings/state/facade.settings';
import {Settings } from '../../../settings/models/settings';
import { ShowPoster } from '../../models/show-poster';
import { ConfigHelper } from '../../helpers/configHelper';

@Component({
  selector: 'app-show-poster',
  templateUrl: './show-poster.component.html',
  styleUrls: ['./show-poster.component.scss']
})
export class ShowPosterComponent implements OnDestroy {
  settingsSub: Subscription;
  settings: Settings;
  @Input() poster: ShowPoster;

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
    return this.sanitizer.bypassSecurityTrustStyle(`url(${fullAddress}/emby/Items/${this.poster.mediaId}/Images/Primary?maxHeight=350&tag=${this.poster.tag}&quality=90)`);
  }

  openShow(): void {
    window.open(`${ConfigHelper.getFullEmbyAddress(this.settings)}/web/index.html#!/itemdetails.html?id=${this.poster.mediaId}`, '_blank');
  }

  ngOnDestroy(): void {
    if (this.settingsSub !== undefined) {
      this.settingsSub.unsubscribe();
    }
  }
}
