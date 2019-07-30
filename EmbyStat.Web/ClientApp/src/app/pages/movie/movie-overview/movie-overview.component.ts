import { NgScrollbar } from 'ngx-scrollbar';
import { Observable, Subscription } from 'rxjs';
import { OptionsService } from 'src/app/shared/components/charts/options/options';

import {
  Component, HostListener, OnDestroy, OnInit, ViewChild
} from '@angular/core';
import { FormControl } from '@angular/forms';
import { MatDialog } from '@angular/material';

import { Options } from '../../../shared/components/charts/options/options';
import { NoTypeFoundDialog } from '../../../shared/dialogs/no-type-found/no-type-found.component';
import { SettingsFacade } from '../../../shared/facades/settings.facade';
import { ConfigHelper } from '../../../shared/helpers/config-helper';
import { Collection } from '../../../shared/models/collection';
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

  selectedCollectionSub: Subscription;
  dropdownBlurredSub: Subscription;
  settingsSub: Subscription;
  isMovieTypePresentSub: Subscription;
  collections$: Observable<Collection[]>;
  collectionsFormControl = new FormControl('', { updateOn: 'blur' });
  typeIsPresent: boolean;

  settings: Settings;

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

    this.collections$ = this.movieService.getCollections();
    this.barOptions = this.optionsService.getBarOptions();

    this.statistics$ = this.movieService.getStatistics([]);

    this.collectionsFormControl.valueChanges.subscribe((collectionList: string[]) => {
      this.statistics$ = this.movieService.getStatistics(collectionList);
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
    window.open(`${embyUrl}/web/index.html#!/item/item.html?id=${id}`, '_blank');
  }

  ngOnDestroy() {
    if (this.selectedCollectionSub !== undefined) {
      this.selectedCollectionSub.unsubscribe();
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
  }
}
