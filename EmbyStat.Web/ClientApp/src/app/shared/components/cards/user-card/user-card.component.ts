import { Subscription } from 'rxjs';

import { Component, EventEmitter, Input, OnDestroy, Output } from '@angular/core';

import { SettingsFacade } from '../../../facades/settings.facade';
import { ConfigHelper } from '../../../helpers/config-helper';
import { MediaServerUser } from '../../../models/media-server/media-server-user';
import { Settings } from '../../../models/settings/settings';

@Component({
  selector: 'es-user-card',
  templateUrl: './user-card.component.html',
  styleUrls: ['./user-card.component.scss']
})
export class UserCardComponent implements OnDestroy {
  private settingsSub: Subscription;
  private settings: Settings;

  @Input() user: MediaServerUser;
  @Output() clicked = new EventEmitter<string>();

  constructor(private readonly settingsFacade: SettingsFacade) {
    this.settingsSub = settingsFacade.getSettings().subscribe(data => this.settings = data);
  }

  getUrlProfileImageUrl(): string {
    const url = ConfigHelper.getFullEmbyAddress(this.settings);
    return `${url}/emby/users/${this.user.id}/images/primary?width=100&tag=${this.user.primaryImageTag}&quality=90`;
  }

  ngOnDestroy() {
    if (this.settingsSub !== undefined) {
      this.settingsSub.unsubscribe();
    }
  }

  openUser(): void {
    this.clicked.emit(this.user.id);
  }
}
