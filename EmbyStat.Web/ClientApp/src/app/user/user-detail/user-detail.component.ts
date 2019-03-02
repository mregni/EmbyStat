import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import * as moment from 'moment';

import { SettingsFacade } from '../../settings/state/facade.settings';
import { Settings } from '../../settings/models/settings';
import { ConfigHelper } from '../../shared/helpers/configHelper';
import { EmbyUser } from '../../shared/models/emby/emby-user';

import { UserService } from '../services/user.service';

@Component({
  selector: 'user-detail',
  templateUrl: './user-detail.component.html',
  styleUrls: ['./user-detail.component.scss']
})
export class UserDetailComponent implements OnInit, OnDestroy {
  private settingsSub: Subscription;
  private settings: Settings;

  displayedColumns: string[] = ['logo', 'name', 'duration', 'start', 'percentage', 'id'];
  user: EmbyUser;

  constructor(
    private readonly settingsFacade: SettingsFacade,
    private readonly userService: UserService) {
    this.settingsSub = settingsFacade.getSettings().subscribe(data => this.settings = data);

    this.userService.user.subscribe((user: EmbyUser) => {
      console.log(user);
      this.user = user;
    });
  }

  ngOnInit() {
  }

  getEmbyAddress(): string {
    return ConfigHelper.getFullEmbyAddress(this.settings);
  }

  getPlayedTime(value: number): string {
    var duration = moment.duration(value, "seconds");
    const hours = this.addLeadingZero(duration.hours());
    const minutes = this.addLeadingZero(duration.minutes());
    const seconds = this.addLeadingZero(duration.seconds());
    return hours + ":" + minutes + ":" + seconds;
  }

  isNowPlaying(value: Date): boolean {
    return moment(value).add(3, "minute").isAfter(moment.now());
  }

  private addLeadingZero(value: number): string {
    if (value.toString().length === 0) {
      return "00";
    } else if (value.toString().length === 1) {
      return "0" + value;
    }

    return value.toString();
  }

  ngOnDestroy(): void {
    if (this.settingsSub !== undefined) {
      this.settingsSub.unsubscribe();
    }
  }
}
