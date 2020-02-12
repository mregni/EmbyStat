import { Subscription } from 'rxjs';
import { EmbyServerInfoFacade } from 'src/app/shared/facades/emby-server.facade';
import { ConfigHelper } from 'src/app/shared/helpers/config-helper';
import { ServerInfo } from 'src/app/shared/models/media-server/server-info';

import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';

import { SettingsFacade } from '../../../facades/settings.facade';
import { Settings } from '../../../models/settings/settings';
import { ShowPoster } from '../../../models/show/show-poster';

@Component({
  selector: 'app-show-poster',
  templateUrl: './show-poster.component.html',
  styleUrls: ['./show-poster.component.scss']
})
export class ShowPosterComponent implements OnInit, OnDestroy {
  settingsSub: Subscription;
  settings: Settings;
  @Input() poster: ShowPoster;

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

    const fullAddress = ConfigHelper.getFullEmbyAddress(this.settings);
    const url = `url(${fullAddress}/emby/Items/${this.poster.mediaId}/Images/Primary?maxHeight=350&tag=${this.poster.tag}&quality=90&enableimageenhancers=false)`;
    return this.sanitizer.bypassSecurityTrustStyle(url);
  }

  openShow() {
    window.open(`${ConfigHelper.getFullEmbyAddress(this.settings)}/web/index.html#!/item/item.html?id=${this.poster.mediaId}&serverId=${this.embyServerInfo.id}`, '_blank');
  }

  ngOnDestroy() {
    if (this.embyServerInfoSub !== undefined) {
      this.embyServerInfoSub.unsubscribe();
    }
  }
}
