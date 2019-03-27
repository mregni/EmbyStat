import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { merge, Observable, Subscription, of as observableOf } from 'rxjs';
import * as moment from 'moment';
import { MatPaginator, MatTableDataSource } from '@angular/material';
import { catchError, map, startWith, switchMap } from 'rxjs/operators';

import { EmbyService } from '../../shared/services/emby.service';
import { PageService } from '../services/page.service';
import { SettingsFacade } from '../../settings/state/facade.settings';
import { Settings } from '../../settings/models/settings';
import { ConfigHelper } from '../../shared/helpers/configHelper';
import { UserMediaView } from '../../shared/models/session/user-media-view';
import { ListContainer } from '../../shared/models/list-container';

@Component({
  selector: 'user-views-detail',
  templateUrl: './user-views-detail.component.html',
  styleUrls: ['./user-views-detail.component.scss']
})
export class UserViewsDetailComponent implements OnInit, OnDestroy {
  private paramSub: Subscription;
  private settingsSub: Subscription;
  private settings: Settings;

  dataSource: MatTableDataSource<UserMediaView>;
  displayedColumns: string[] = ['logo', 'name', 'duration', 'start', 'percentage', 'id'];

  @ViewChild(MatPaginator) paginator: MatPaginator;

  constructor(private readonly activatedRoute: ActivatedRoute,
    private readonly router: Router,
    private readonly embyService: EmbyService,
    private readonly settingsFacade: SettingsFacade,
    private readonly pageService: PageService) {
    this.settingsSub = settingsFacade.getSettings().subscribe(data => this.settings = data);
    this.pageService.pageChanged('views');


    this.paramSub = this.activatedRoute.parent.params.subscribe(params => {
      const id = params['id'];
      if (!!id) {
        this.dataSource = new MatTableDataSource([]);
        this.dataSource.paginator = this.paginator;

        window.setTimeout(() => {
          merge(this.paginator.page)
            .pipe(
              startWith({}),
              switchMap(() => {
                return this.embyService.getUserViewsByUserId(id, this.paginator.pageIndex, this.paginator.pageSize);
              }),
              map((data: ListContainer<UserMediaView>) => {
                console.log(data);
                this.paginator.length = data.totalCount;
                return data.data;
              }),
              catchError(() => {
                return observableOf([]);
              })
            ).subscribe((list: UserMediaView[]) => {
              this.dataSource = new MatTableDataSource(list);
            });
        }, 10);
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
