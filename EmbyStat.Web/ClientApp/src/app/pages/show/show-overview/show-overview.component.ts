import * as _ from 'lodash';
import { NgScrollbar } from 'ngx-scrollbar';
import { Observable, Subscription } from 'rxjs';
import { EmbyServerInfoFacade } from 'src/app/shared/facades/emby-server.facade';
import { ConfigHelper } from 'src/app/shared/helpers/config-helper';
import { ServerInfo } from 'src/app/shared/models/media-server/server-info';

import { animate, state, style, transition, trigger } from '@angular/animations';
import {
    AfterViewInit, Component, HostListener, OnDestroy, OnInit, ViewChild, ViewChildren
} from '@angular/core';
import { FormControl } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { Sort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { DomSanitizer, SafeStyle } from '@angular/platform-browser';

import { Options, OptionsService } from '../../../shared/components/charts/options/options';
import { NoTypeFoundDialog } from '../../../shared/dialogs/no-type-found/no-type-found.component';
import { SettingsFacade } from '../../../shared/facades/settings.facade';
import { Library } from '../../../shared/models/library';
import { ListContainer } from '../../../shared/models/list-container';
import { Settings } from '../../../shared/models/settings/settings';
import { ShowCollectionRow } from '../../../shared/models/show/show-collection-row';
import { ShowStatistics } from '../../../shared/models/show/show-statistics';
import { ShowService } from '../service/show.service';

@Component({
  selector: 'es-show-overview',
  templateUrl: './show-overview.component.html',
  styleUrls: ['./show-overview.component.scss'],
  animations: [
    trigger('detailExpand', [
      state('collapsed', style({ height: '0px', minHeight: '0' })),
      state('expanded', style({ height: '*' })),
      transition('expanded <=> collapsed', animate('225ms cubic-bezier(0.4, 0.0, 0.2, 1)')),
    ]),
  ]
})
export class ShowOverviewComponent implements OnInit, OnDestroy, AfterViewInit {
  statistics$: Observable<ShowStatistics>;
  rows: ShowCollectionRow[];
  sortedRowsDataSource: MatTableDataSource<ShowCollectionRow>;

  settingsSub: Subscription;
  resizeSub: Subscription;
  collectedDataSub: Subscription;
  isShowTypePresentSub: Subscription;
  paginatorPageSub: Subscription;
  libraries$: Observable<Library[]>;
  librariesFormControl = new FormControl('', { updateOn: 'blur' });

  typeIsPresent = false;
  settings: Settings;

  pieOptions: Options;
  barOptions: Options;

  displayedColumnsWide = ['title', 'premiereDate', 'status', 'seasons', 'precentage'];
  displayedColumnsSmall = ['title', 'status', 'precentage'];

  embyServerInfo: ServerInfo;
  embyServerInfoSub: Subscription;

  showDetails = [];
  showCount: number;
  selectedCollectionList: string[];

  expandedRow: ShowCollectionRow;

  private paginator: MatPaginator;
  @ViewChild(MatPaginator) set pane(mp: MatPaginator) {
    this.paginator = mp;
    if (mp !== undefined) {
      this.paginatorPageSub = this.paginator.page.subscribe(() => {
        this.collectedDataSub = this.showService.getCollectedList(this.selectedCollectionList, this.paginator.pageIndex)
          .subscribe((pageData: ListContainer<ShowCollectionRow>) => {
            this.setShowTable(pageData);
          });
      });
    }
  }

  @ViewChild(NgScrollbar) textAreaScrollbar: NgScrollbar;

  constructor(
    private readonly showService: ShowService,
    private readonly settingsFacade: SettingsFacade,
    private readonly embyServerInfoFacade: EmbyServerInfoFacade,
    public dialog: MatDialog,
    private readonly optionsService: OptionsService,
    private readonly sanitizer: DomSanitizer) {
    this.settingsSub = this.settingsFacade.getSettings().subscribe((settings: Settings) => {
      this.settings = settings;
    });

    this.isShowTypePresentSub = this.showService.isTypePresent().subscribe((typePresent: boolean) => {
      this.typeIsPresent = typePresent;
      if (!typePresent) {
        this.dialog.open(NoTypeFoundDialog,
          {
            width: '550px',
            data: 'SHOWS'
          });
      }
    });

    this.libraries$ = this.showService.getLibraries();
    this.statistics$ = this.showService.getStatistics([]);

    this.pieOptions = this.optionsService.getPieOptions();
    this.barOptions = this.optionsService.getBarOptions();

    this.selectedCollectionList = [];

    this.librariesFormControl.valueChanges.subscribe((collectionList: string[]) => {
      this.selectedCollectionList = collectionList;
      this.statistics$ = this.showService.getStatistics(collectionList);
      this.collectedDataSub = this.showService.getCollectedList(this.selectedCollectionList, 0)
        .subscribe((data: ListContainer<ShowCollectionRow>) => {
          this.setShowTable(data);
        });
    });

    this.embyServerInfoSub = this.embyServerInfoFacade.getEmbyServerInfo().subscribe((info: ServerInfo) => {
      this.embyServerInfo = info;
    });
  }

  ngOnInit() {
    this.sortedRowsDataSource = new MatTableDataSource([]);
  }

  ngAfterViewInit() {
    this.collectedDataSub = this.showService.getCollectedList(this.selectedCollectionList, 0)
      .subscribe((data: ListContainer<ShowCollectionRow>) => {
        this.setShowTable(data);
      });
  }

  openShow(id: string): void {
    const embyUrl = ConfigHelper.getFullEmbyAddress(this.settings);
    window.open(`${embyUrl}/web/index.html#!/item/item.html?id=${id}&serverId=${this.embyServerInfo.id}`, '_blank');
  }

  openImdb(id: string): void {
    window.open('https://www.imdb.com/title/' + id);
  }

  openTvdb(id: string): void {
    window.open('https://thetvdb.com/?tab=series&id=' + id);
  }

  getcolumns(): string[] {
    return window.window.innerWidth > 720 ? this.displayedColumnsWide : this.displayedColumnsSmall;
  }

  getShowBannerLink(show: ShowCollectionRow): string {
    const fullAddress = ConfigHelper.getFullEmbyAddress(this.settings);
    return `${fullAddress}/emby/Items/${show.id}/Images/Banner?maxHeight=60&tag=${show.banner}&quality=90&enableimageenhancers=false`;
  }

  getColor(row: ShowCollectionRow): string {
    const percentage = this.calculatePercentage(row) * 100;
    if (percentage === 100) {
      return '#5B990D';
    } else if (percentage >= 80) {
      return '#9DB269';
    } else if (percentage >= 60) {
      return '#F2A70D';
    } else if (percentage >= 40) {
      return '#F2700D';
    } else {
      return '#B11A10';
    }
  }

  calculatePercentage(row: ShowCollectionRow): number {
    if (row.episodes + row.missingEpisodeCount === 0) {
      return 0;
    } else {
      return row.episodes / (row.episodes + row.missingEpisodeCount);
    }
  }

  @HostListener('window:resize', ['$event'])
  onResize(event) {
    this.resizeSub = this.statistics$.subscribe(() => {
      this.textAreaScrollbar.update();
    });
  }

  private setShowTable(data: ListContainer<ShowCollectionRow>) {
    this.rows = data.data;
    this.sortedRowsDataSource.data = data.data;
    this.showCount = data.totalCount;
  }

  ngOnDestroy() {
    if (this.settingsSub !== undefined) {
      this.settingsSub.unsubscribe();
    }

    if (this.resizeSub !== undefined) {
      this.resizeSub.unsubscribe();
    }

    if (this.isShowTypePresentSub !== undefined) {
      this.isShowTypePresentSub.unsubscribe();
    }

    if (this.collectedDataSub !== undefined) {
      this.collectedDataSub.unsubscribe();
    }

    if (this.embyServerInfoSub !== undefined) {
      this.embyServerInfoSub.unsubscribe();
    }

    if (this.paginatorPageSub !== undefined) {
      this.paginatorPageSub.unsubscribe();
    }
  }
}
