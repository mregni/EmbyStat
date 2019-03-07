import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Observable, Subscription } from 'rxjs';
import * as moment from 'moment';

import { EmbyService } from '../../shared/services/emby.service';
import { SettingsFacade } from '../../settings/state/facade.settings';
import { Settings } from '../../settings/models/settings';
import { ConfigHelper } from '../../shared/helpers/configHelper';
import { UserMediaView } from '../../shared/models/session/user-media-view';

@Component({
  selector: 'user-views-detail',
  templateUrl: './user-views-detail.component.html',
  styleUrls: ['./user-views-detail.component.scss']
})
export class UserViewsDetailComponent implements OnInit, OnDestroy {
  private paramSub: Subscription;
  private settingsSub: Subscription;
  private settings: Settings;

  displayedColumns: string[] = ['logo', 'name', 'duration', 'start', 'percentage', 'id'];
  views$: Observable<UserMediaView[]>;
  username: string;

  constructor(private readonly activatedRoute: ActivatedRoute,
    private readonly router: Router,
    private readonly embyService: EmbyService,
    private readonly settingsFacade: SettingsFacade) {
    this.settingsSub = settingsFacade.getSettings().subscribe(data => this.settings = data);

    this.paramSub = this.activatedRoute.parent.params.subscribe(params => {
      const id = params['id'];
      if (!!id) {
        this.views$ = this.embyService.getUserViewsByUserId(id);
      } else {
        this.router.navigate(['/users']);
      }
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
    if (this.paramSub !== undefined) {
      this.paramSub.unsubscribe();
    }

    if (this.settingsSub !== undefined) {
      this.settingsSub.unsubscribe();
    }
  }
}
