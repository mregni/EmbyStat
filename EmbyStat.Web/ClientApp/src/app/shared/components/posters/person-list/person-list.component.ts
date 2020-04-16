import { Subscription } from 'rxjs';
import { EmbyServerInfoFacade } from 'src/app/shared/facades/emby-server.facade';
import { ConfigHelper } from 'src/app/shared/helpers/config-helper';
import { ServerInfo } from 'src/app/shared/models/media-server/server-info';

import { Component, Input, OnDestroy } from '@angular/core';

import { SettingsFacade } from '../../../facades/settings.facade';
import { PersonPoster } from '../../../models/common/person-poster';
import { Settings } from '../../../models/settings/settings';

@Component({
  selector: 'es-person-list',
  templateUrl: './person-list.component.html',
  styleUrls: ['./person-list.component.scss']
})
export class PersonListComponent implements OnDestroy {
  settingsSub: Subscription;
  settings: Settings;
  @Input() posters: PersonPoster[];

  embyServerInfo: ServerInfo;
  embyServerInfoSub: Subscription;

  opts = {
    position: 'right',
    alwaysVisible: true
  };

  constructor(
    private settingsFacade: SettingsFacade,
    private readonly embyServerInfoFacade: EmbyServerInfoFacade) {
    this.settingsSub = settingsFacade.getSettings().subscribe(data => this.settings = data);

    this.embyServerInfoSub = this.embyServerInfoFacade.getEmbyServerInfo().subscribe((info: ServerInfo) => {
      this.embyServerInfo = info;
    });
  }

  getPoster(tag: string, mediaId: string): string {
    if (this.settings === undefined) {
      return '';
    }

    if (tag === null) {
      return '../../../../assets/images/backgrounds/defaultPerson.png';
    }

    const fullAddress = ConfigHelper.getFullEmbyAddress(this.settings);
    return `${fullAddress}/emby/Items/${mediaId}/Images/Primary?maxHeight=350&tag=${tag}&quality=90&enableimageenhancers=false`;
  }

  openPerson(mediaId: string): void {
    window.open(`${ConfigHelper.getFullEmbyAddress(this.settings)}/web/index.html#!/item/item.html?id=${mediaId}&serverId=${this.embyServerInfo.id}`, '_blank');
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
