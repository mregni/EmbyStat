import * as moment from 'moment';
import { Subscription } from 'rxjs';

import { Component, OnDestroy, OnInit } from '@angular/core';

import { SettingsFacade } from '../../../../shared/facades/settings.facade';
import { ConfigHelper } from '../../../../shared/helpers/config-helper';
import { EmbyUser } from '../../../../shared/models/emby/emby-user';
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
  user: EmbyUser;

  constructor(
    private readonly settingsFacade: SettingsFacade,
    private readonly userService: UserService,
    private readonly pageService: PageService) {
    this.settingsSub = settingsFacade.getSettings().subscribe(data => this.settings = data);

    this.userService.user.subscribe((user: EmbyUser) => {
      this.user = user;
    });
  }

  ngOnInit() {
    this.pageService.pageChanged('detail');
  }

  getEmbyAddress(): string {
    return ConfigHelper.getFullEmbyAddress(this.settings);
  }

  getcolumns(): string[] {
    return window.window.innerWidth > 720 ? this.displayedColumnsWide : this.displayedColumnsSmall;
  }

  getPlayedTime(value: number): string {
    const duration = moment.duration(value, 'seconds');
    const hours = this.addLeadingZero(duration.hours());
    const minutes = this.addLeadingZero(duration.minutes());
    const seconds = this.addLeadingZero(duration.seconds());
    return hours + ':' + minutes + ':' + seconds;
  }

  isNowPlaying(value: Date): boolean {
    return moment(value).add(3, 'minute').isAfter(moment.now());
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
  }
}
