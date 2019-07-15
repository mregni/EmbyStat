import { Subscription } from 'rxjs';

import { Component, EventEmitter, Input, OnDestroy, Output } from '@angular/core';

import { SettingsFacade } from '../../../facades/settings.facade';
import { ConfigHelper } from '../../../helpers/config-helper';
import { EmbyUser } from '../../../models/emby/emby-user';
import { Settings } from '../../../models/settings/settings';

@Component({
  selector: 'app-user-card',
  templateUrl: './user-card.component.html',
  styleUrls: ['./user-card.component.scss']
})
export class UserCardComponent implements OnDestroy {
  private settingsSub: Subscription;
  private settings: Settings;

  @Input() user: EmbyUser;
  @Output() clicked = new EventEmitter<string>();

  constructor(private readonly settingsFacade: SettingsFacade) {
    this.settingsSub = settingsFacade.getSettings().subscribe(data => this.settings = data);
  }

  getEmbyAddress(): string {
    return ConfigHelper.getFullEmbyAddress(this.settings);
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
