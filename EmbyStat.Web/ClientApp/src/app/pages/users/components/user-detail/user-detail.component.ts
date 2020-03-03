import * as moment from 'moment';
import { Subscription } from 'rxjs';
import { EmbyServerInfoFacade } from 'src/app/shared/facades/emby-server.facade';
import { ServerInfo } from 'src/app/shared/models/media-server/server-info';

import { Component, OnDestroy, OnInit } from '@angular/core';

import { SettingsFacade } from '../../../../shared/facades/settings.facade';
import { ConfigHelper } from '../../../../shared/helpers/config-helper';
import { MediaServerUser } from '../../../../shared/models/media-server/media-server-user';
import { UserMediaView } from '../../../../shared/models/session/user-media-view';
import { Settings } from '../../../../shared/models/settings/settings';
import { PageService } from '../../../../shared/services/page.service';
import { UserService } from '../../../../shared/services/user.service';

@Component({
  selector: 'app-user-detail',
  templateUrl: './user-detail.component.html',
  styleUrls: ['./user-detail.component.scss']
})
export class UserDetailComponent implements OnInit, OnDestroy {
  private settingsSub: Subscription;
  private settings: Settings;

  displayedColumnsWide: string[] = ['logo', 'name', 'duration', 'start', 'percentage', 'id'];
  displayedColumnsSmall: string[] = ['logo', 'name', 'percentage', 'id'];
  user: MediaServerUser;

  embyServerInfo: ServerInfo;
  embyServerInfoSub: Subscription;

  constructor(
    private readonly settingsFacade: SettingsFacade,
    private readonly embyServerInfoFacade: EmbyServerInfoFacade,
    private readonly userService: UserService,
    private readonly pageService: PageService) {
    this.settingsSub = settingsFacade.getSettings().subscribe(data => this.settings = data);

    this.userService.user.subscribe((user: MediaServerUser) => {
      this.user = user;
    });

    this.embyServerInfoSub = this.embyServerInfoFacade.getEmbyServerInfo().subscribe((info: ServerInfo) => {
      this.embyServerInfo = info;
    });
  }

  ngOnInit() {
    this.pageService.pageChanged('detail');
  }

  getcolumns(): string[] {
    return window.window.innerWidth > 720 ? this.displayedColumnsWide : this.displayedColumnsSmall;
  }

  getPlayedTime(value: number): string {
    const duration = moment.duration(value, 'seconds');
    const hours = this.addLeadingZero(duration.hours());
    const minutes = this.addLeadingZero(duration.minutes());
    const seconds = this.addLeadingZero(duration.seconds());
    return `${hours}:${minutes}:${seconds}`;
  }

  isNowPlaying(value: Date): boolean {
    return moment(value).add(3, 'minute').isAfter(moment.now());
  }

  getUrlProfileImageUrl(): string {
    const url = ConfigHelper.getFullEmbyAddress(this.settings);
    return `${url}/emby/users/${this.user.id}/images
            /primary?width=100&tag=${this.user.primaryImageTag}&quality=90`;
  }

  getItemImageUrl(item: UserMediaView): string {
    const url = ConfigHelper.getFullEmbyAddress(this.settings);
    return `${url}/web/index.html#!/item/item.html?id=${item.id}&serverId=${this.embyServerInfo.id}`;
  }

  private addLeadingZero(value: number): string {
    if (value.toString().length === 0) {
      return '00';
    } else if (value.toString().length === 1) {
      return '0' + value;
    }

    return value.toString();
  }

  ngOnDestroy(): void {
    if (this.settingsSub !== undefined) {
      this.settingsSub.unsubscribe();
    }

    if (this.embyServerInfoSub !== undefined) {
      this.embyServerInfoSub.unsubscribe();
    }
  }
}
