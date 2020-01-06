import * as moment from 'moment';
import { merge, Observable, of as observableOf, Subscription } from 'rxjs';
import { catchError, map, startWith, switchMap } from 'rxjs/operators';
import { EmbyServerInfoFacade } from 'src/app/shared/facades/emby-server.facade';
import { ServerInfo } from 'src/app/shared/models/emby/server-info';

import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatTableDataSource } from '@angular/material/table';
import { ActivatedRoute, Router } from '@angular/router';

import { SettingsFacade } from '../../../../shared/facades/settings.facade';
import { ConfigHelper } from '../../../../shared/helpers/config-helper';
import { ListContainer } from '../../../../shared/models/list-container';
import { UserMediaView } from '../../../../shared/models/session/user-media-view';
import { Settings } from '../../../../shared/models/settings/settings';
import { EmbyService } from '../../../../shared/services/emby.service';
import { PageService } from '../../../../shared/services/page.service';

@Component({
  selector: 'app-user-views-detail',
  templateUrl: './user-views-detail.component.html',
  styleUrls: ['./user-views-detail.component.scss']
})
export class UserViewsDetailComponent implements OnInit, OnDestroy {
  private paramSub: Subscription;
  private settingsSub: Subscription;
  private settings: Settings;

  dataSource: MatTableDataSource<UserMediaView>;
  displayedColumnsWide: string[] = ['logo', 'name', 'duration', 'start', 'percentage', 'id'];
  displayedColumnsSmall: string[] = ['logo', 'name', 'percentage', 'id'];

  embyServerInfo: ServerInfo;
  embyServerInfoSub: Subscription;

  @ViewChild(MatPaginator, {static: false}) paginator: MatPaginator;

  constructor(private readonly activatedRoute: ActivatedRoute,
    private readonly router: Router,
    private readonly embyService: EmbyService,
    private readonly embyServerInfoFacade: EmbyServerInfoFacade,
    private readonly settingsFacade: SettingsFacade,
    private readonly pageService: PageService) {
    this.settingsSub = settingsFacade.getSettings().subscribe(data => this.settings = data);
    this.pageService.pageChanged('views');

    this.embyServerInfoSub = this.embyServerInfoFacade.getEmbyServerInfo().subscribe((info: ServerInfo) => {
      this.embyServerInfo = info;
    });

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

  getcolumns(): string[] {
    return window.window.innerWidth > 720 ? this.displayedColumnsWide : this.displayedColumnsSmall;
  }

  getEmbyAddress(): string {
    return ConfigHelper.getFullEmbyAddress(this.settings);
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

  private addLeadingZero(value: number): string {
    if (value.toString().length === 0) {
      return '00';
    } else if (value.toString().length === 1) {
      return '0' + value;
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

    if (this.embyServerInfoSub !== undefined) {
      this.embyServerInfoSub.unsubscribe();
    }
  }
}
