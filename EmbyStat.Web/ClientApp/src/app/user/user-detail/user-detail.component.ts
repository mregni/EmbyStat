import { Component, OnInit, OnDestroy, AfterViewInit, ViewChild, ElementRef } from '@angular/core';
import { Subscription } from 'rxjs';
import * as moment from 'moment';
import { Chart } from 'chart.js';

import { SettingsFacade } from '../../settings/state/facade.settings';
import { Settings } from '../../settings/models/settings';
import { ConfigHelper } from '../../shared/helpers/configHelper';
import { EmbyUser } from '../../shared/models/emby/emby-user';

import { UserService } from '../services/user.service';
import { PageService } from '../services/page.service';

@Component({
  selector: 'user-detail',
  templateUrl: './user-detail.component.html',
  styleUrls: ['./user-detail.component.scss']
})
export class UserDetailComponent implements OnInit, OnDestroy, AfterViewInit {
  private settingsSub: Subscription;
  private settings: Settings;

  displayedColumns: string[] = ['logo', 'name', 'duration', 'start', 'percentage', 'id'];
  user: EmbyUser;
  hoursPerDayGraph = [];
  @ViewChild('hoursPerDayGraphRef') hoursPerDayGraphRef: ElementRef;

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
    this.pageService.pageChanged('details');
  }

  ngAfterViewInit() {
    console.log(this.user.hoursPerDayGraph);
    this.hoursPerDayGraph = new Chart(this.hoursPerDayGraphRef.nativeElement.getContext('2d')
      , {
        type: 'bar',
        data: this.user.hoursPerDayGraph,
        options: {
          responsive: true,
          legend: {
            display: false
          },
          scales: {
            xAxes: [{
              display: true
            }],
            yAxes: [{
              display: true,
              ticks: {
                callback: (value, index, values) => value + ' %'
              }
            }],
          },
          title: {
            display: false
          },
          tooltips: {
            callbacks: {
              label: function (tooltipItem, data) {
                var label = data.datasets[tooltipItem.datasetIndex].label || '';

                if (label) {
                  label += ': ';
                }
                label += Math.round(tooltipItem.yLabel * 100) / 100 + ' %';
                return label;
              }
            }
          }
        }
      });
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
