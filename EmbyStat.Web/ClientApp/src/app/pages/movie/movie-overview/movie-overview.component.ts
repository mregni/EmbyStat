import { NgScrollbar } from 'ngx-scrollbar';
import { Observable, Subscription } from 'rxjs';
import { OptionsService } from 'src/app/shared/components/charts/options/options';
import { EmbyServerInfoFacade } from 'src/app/shared/facades/emby-server.facade';
import { ServerInfo } from 'src/app/shared/models/emby/server-info';

import { Component, HostListener, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { FormControl } from '@angular/forms';
import { MatDialog } from '@angular/material';

import { Options } from '../../../shared/components/charts/options/options';
import { NoTypeFoundDialog } from '../../../shared/dialogs/no-type-found/no-type-found.component';
import { SettingsFacade } from '../../../shared/facades/settings.facade';
import { ConfigHelper } from '../../../shared/helpers/config-helper';
import { Library } from '../../../shared/models/library';
import { MovieStatistics } from '../../../shared/models/movie/movie-statistics';
import { Settings } from '../../../shared/models/settings/settings';
import { MovieService } from '../service/movie.service';

@Component({
  selector: 'app-movie-overview',
  templateUrl: './movie-overview.component.html',
  styleUrls: ['./movie-overview.component.scss']
})
export class MovieOverviewComponent implements OnInit, OnDestroy {
  statistics$: Observable<MovieStatistics>;

  @ViewChild(NgScrollbar)
  textAreaScrollbar: NgScrollbar;

  selectedLibrarySub: Subscription;
  dropdownBlurredSub: Subscription;
  settingsSub: Subscription;
  isMovieTypePresentSub: Subscription;
  libraries$: Observable<Library[]>;
  librariesFormControl = new FormControl('', { updateOn: 'blur' });
  typeIsPresent: boolean;

  settings: Settings;

  embyServerInfo: ServerInfo;
  embyServerInfoSub: Subscription;

  suspiciousDisplayedWideColumns = [
    'position', 'title', 'reason', 'linkOne', 'qualityOne', 'addedOnOne', 'linkTwo', 'qualityTwo', 'addedOnTwo'
  ];
  suspiciousDisplayedSmallColumns = [
    'position', 'title', 'reason', 'linkOne', 'linkTwo'
  ];
  shortDisplayedColumns = ['number', 'title', 'duration', 'link'];
  noImdbDisplayedColumns = ['number', 'title', 'link'];
  noPrimaryDisplayedColumns = ['number', 'title', 'link'];

  barOptions: Options;

  constructor(
    private readonly settingsFacade: SettingsFacade,
    private readonly movieService: MovieService,
    private readonly embyServerInfoFacade: EmbyServerInfoFacade,
    public dialog: MatDialog,
    private readonly optionsService: OptionsService) {
    this.settingsSub = this.settingsFacade.getSettings().subscribe((settings: Settings) => {
      this.settings = settings;
    });

    this.isMovieTypePresentSub = this.movieService.isTypePresent().subscribe((typePresent: boolean) => {
      this.typeIsPresent = typePresent;
      if (!typePresent) {
        this.dialog.open(NoTypeFoundDialog,
          {
            width: '550px',
            data: 'MOVIES'
          });
      }
    });

    this.libraries$ = this.movieService.getLibraries();
    this.barOptions = this.optionsService.getBarOptions();

    this.statistics$ = this.movieService.getStatistics([]);

    this.librariesFormControl.valueChanges.subscribe((libraryList: string[]) => {
      this.statistics$ = this.movieService.getStatistics(libraryList);
    });

    this.embyServerInfoSub = this.embyServerInfoFacade.getEmbyServerInfo().subscribe((info: ServerInfo) => {
      this.embyServerInfo = info;
    });
  }

  ngOnInit() {
  }

  getSuspiciousColumns(): string[] {
    return window.window.innerWidth > 720 ? this.suspiciousDisplayedWideColumns : this.suspiciousDisplayedSmallColumns;
  }

  @HostListener('window:resize', ['$event'])
  onResize(event) {
    this.statistics$.subscribe(() => {
      this.textAreaScrollbar.update();
    });
  }

  openMovie(id: string): void {
    const embyUrl = ConfigHelper.getFullEmbyAddress(this.settings);
    window.open(`${embyUrl}/web/index.html#!/item/item.html?id=${id}&serverId=${this.embyServerInfo.id}`, '_blank');
  }

  ngOnDestroy() {
    if (this.selectedLibrarySub !== undefined) {
      this.selectedLibrarySub.unsubscribe();
    }

    if (this.dropdownBlurredSub !== undefined) {
      this.dropdownBlurredSub.unsubscribe();
    }

    if (this.settingsSub !== undefined) {
      this.settingsSub.unsubscribe();
    }

    if (this.isMovieTypePresentSub !== undefined) {
      this.isMovieTypePresentSub.unsubscribe();
    }

    if (this.embyServerInfoSub !== undefined) {
      this.embyServerInfoSub.unsubscribe();
    }
  }
}
