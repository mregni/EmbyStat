import { Subscription } from 'rxjs';
import { ConfigHelper } from 'src/app/shared/helpers/config-helper';

import { Component, Input, OnInit } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';

import { SettingsFacade } from '../../../facades/settings.facade';
import { Settings } from '../../../models/settings/settings';
import { ShowPoster } from '../../../models/show/show-poster';

@Component({
  selector: 'app-show-poster',
  templateUrl: './show-poster.component.html',
  styleUrls: ['./show-poster.component.scss']
})
export class ShowPosterComponent implements OnInit {
  settingsSub: Subscription;
  settings: Settings;
  @Input() poster: ShowPoster;

  constructor(
    private readonly settingsFacade: SettingsFacade,
    private readonly sanitizer: DomSanitizer) {
    this.settingsSub = this.settingsFacade.getSettings().subscribe(data => this.settings = data);
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

  openShow() {
    window.open(`${ConfigHelper.getFullEmbyAddress(this.settings)}/web/index.html#!/itemdetails.html?id=${this.poster.mediaId}`, '_blank');
  }
}
