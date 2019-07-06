import { NgScrollbar } from 'ngx-scrollbar';
import { Observable, Subscription } from 'rxjs';
import { ConfigHelper } from 'src/app/shared/helpers/config-helper';
import { GeneralShowStatistics } from 'src/app/shared/models/show/general-show-statistics';

import { Component, HostListener, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { FormControl } from '@angular/forms';
import { MatDialog, MatSort, MatTableDataSource, Sort } from '@angular/material';

import { Options, OptionsService } from '../../../shared/components/charts/options/options';
import { NoTypeFoundDialog } from '../../../shared/dialogs/no-type-found/no-type-found.component';
import { SettingsFacade } from '../../../shared/facades/settings.facade';
import { Collection } from '../../../shared/models/collection';
import { Settings } from '../../../shared/models/settings/settings';
import { ShowCharts } from '../../../shared/models/show/show-charts';
import { ShowCollectionRow } from '../../../shared/models/show/show-collection-row';
import { ShowService } from '../service/show.service';

@Component({
  selector: 'app-show-overview',
  templateUrl: './show-overview.component.html',
  styleUrls: ['./show-overview.component.scss']
})
export class ShowOverviewComponent implements OnInit, OnDestroy {
  generalStatistics$: Observable<GeneralShowStatistics>;
  charts$: Observable<ShowCharts>;

  rows: ShowCollectionRow[];
  sortedRowsDataSource: MatTableDataSource<ShowCollectionRow>;

  settingsSub: Subscription;
  resizeSub: Subscription;
  collectedDataSub: Subscription;
  isShowTypePresentSub: Subscription;
  collections$: Observable<Collection[]>;
  collectionsFormControl = new FormControl('', { updateOn: 'blur' });

  typeIsPresent = false;
  settings: Settings;

  pieOptions: Options;
  barOptions: Options;

  displayedColumnsWide = ['title', 'premiereDate', 'status', 'seasons', 'precentage'];
  displayedColumnsSmall = ['title', 'status', 'precentage'];

  @ViewChild(NgScrollbar) textAreaScrollbar: NgScrollbar;

  constructor(
    private readonly showService: ShowService,
    private readonly settingsFacade: SettingsFacade,
    public dialog: MatDialog,
    private readonly optionsService: OptionsService) {
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

    this.collections$ = this.showService.getCollections();

    this.generalStatistics$ = this.showService.getGeneralStats([]);
    this.charts$ = this.showService.getCharts([]);

    this.pieOptions = this.optionsService.getPieOptions();
    this.barOptions = this.optionsService.getBarOptions();

    this.collectionsFormControl.valueChanges.subscribe((collectionList: string[]) => {
      this.generalStatistics$ = this.showService.getGeneralStats(collectionList);
      this.charts$ = this.showService.getCharts(collectionList);
    });
  }

  ngOnInit() {
    this.collectedDataSub = this.showService.getCollectedList([]).subscribe((data: ShowCollectionRow[]) => {
      this.rows = data;
      this.sortedRowsDataSource = new MatTableDataSource(data);
    });
  }

  openShow(id: string): void {
    const embyUrl = ConfigHelper.getFullEmbyAddress(this.settings);
    window.open(`${embyUrl}/web/index.html#!/itemdetails.html?id=${id}`, '_blank');
  }

  sortData(sort: Sort) {
    const data = this.rows;
    if (!sort.active || sort.direction === '') {
      this.sortedRowsDataSource.data = data;
      return;
    }

    this.sortedRowsDataSource.data = data.sort((a, b) => {
      const isAsc = sort.direction === 'asc';
      switch (sort.active) {
        case 'title': return this.compare(a.sortName, b.sortName, isAsc);
        case 'premiereDate': return this.compare(a.premiereDate, b.premiereDate, isAsc);
        case 'seasons': return this.compare(a.seasons, b.seasons, isAsc);
        case 'precentage': return this.compare(this.calculatePercentage(a), this.calculatePercentage(b), isAsc);
        default: return 0;
      }
    });
  }

  getcolumns(): string[] {
    return window.window.innerWidth > 720 ? this.displayedColumnsWide : this.displayedColumnsSmall;
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
    if (row.episodes + row.missingEpisodes === 0) {
      return 0;
    } else {
      return row.episodes / (row.episodes + row.missingEpisodes);
    }
  }

  @HostListener('window:resize', ['$event'])
  onResize(event) {
    this.resizeSub = this.generalStatistics$.subscribe(() => {
      this.textAreaScrollbar.update();
    });
  }

  private compare(a: number | string | Date, b: number | string | Date, isAsc: boolean) {
    return (a < b ? -1 : 1) * (isAsc ? 1 : -1);
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
  }
}
