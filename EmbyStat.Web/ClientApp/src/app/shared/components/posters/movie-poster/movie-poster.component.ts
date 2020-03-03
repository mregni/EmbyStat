import { Subscription } from 'rxjs';
import { EmbyServerInfoFacade } from 'src/app/shared/facades/emby-server.facade';
import { SettingsFacade } from 'src/app/shared/facades/settings.facade';
import { ServerInfo } from 'src/app/shared/models/media-server/server-info';

import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';

import { ConfigHelper } from '../../../helpers/config-helper';
import { MoviePoster } from '../../../models/movie/movie-poster';
import { Settings } from '../../../models/settings/settings';

@Component({
  selector: 'app-movie-poster',
  templateUrl: './movie-poster.component.html',
  styleUrls: ['./movie-poster.component.scss']
})
export class MoviePosterComponent implements OnInit, OnDestroy {
  settingsSub: Subscription;
  settings: Settings;
  @Input() poster: MoviePoster;

  embyServerInfo: ServerInfo;
  embyServerInfoSub: Subscription;

  constructor(
    private readonly settingsFacade: SettingsFacade,
    private readonly embyServerInfoFacade: EmbyServerInfoFacade,
    private readonly sanitizer: DomSanitizer) {
    this.settingsSub = this.settingsFacade.getSettings().subscribe(data => this.settings = data);

    this.embyServerInfoSub = this.embyServerInfoFacade.getEmbyServerInfo().subscribe((info: ServerInfo) => {
      this.embyServerInfo = info;
    });
  }

  ngOnInit() {
  }

  getPoster() {
    if (this.settings === undefined) {
      return '';
    }

    if (this.poster.mediaId !== '0') {
      const fullAddress = ConfigHelper.getFullEmbyAddress(this.settings);
      const url = `url(${fullAddress}/emby/Items/${this.poster.mediaId}/Images/Primary?maxHeight=350&tag=${this.poster.tag}&quality=90&enableimageenhancers=false)`;
      return this.sanitizer.bypassSecurityTrustStyle(url);
    }
  }

  openMovie() {
    window.open(`${ConfigHelper.getFullEmbyAddress(this.settings)}/web/index.html#!/item/item.html?id=${this.poster.mediaId}&serverId=${this.embyServerInfo.id}`, '_blank');
  }

  ngOnDestroy() {
    if (this.settingsSub !== undefined) {
      this.settingsSub.unsubscribe();
    }
    if (this.embyServerInfoSub !== undefined) {
      this.embyServerInfoSub.unsubscribe();
    }
  }
}
