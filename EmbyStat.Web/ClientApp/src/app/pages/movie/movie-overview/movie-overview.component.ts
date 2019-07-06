import { NgScrollbar } from 'ngx-scrollbar';
import { Observable, Subscription } from 'rxjs';
import { OptionsService } from 'src/app/shared/components/charts/options/options';
import { PersonStatistics } from 'src/app/shared/models/common/person-statistics';
import { MovieCharts } from 'src/app/shared/models/movie/movie-charts';
import { MovieSuspiciousContainer } from 'src/app/shared/models/movie/movie-suspicious-container';

import {
    AfterViewInit, Component, HostListener, OnDestroy, OnInit, ViewChild
} from '@angular/core';
import { FormControl } from '@angular/forms';
import { MatDialog } from '@angular/material';

import { Options } from '../../../shared/components/charts/options/options';
import { NoTypeFoundDialog } from '../../../shared/dialogs/no-type-found/no-type-found.component';
import { SettingsFacade } from '../../../shared/facades/settings.facade';
import { ConfigHelper } from '../../../shared/helpers/config-helper';
import { Collection } from '../../../shared/models/collection';
import { GeneralMovieStatistics } from '../../../shared/models/movie/general-movie-statistics';
import { Settings } from '../../../shared/models/settings/settings';
import { MovieService } from '../service/movie.service';

@Component({
  selector: 'app-movie-overview',
  templateUrl: './movie-overview.component.html',
  styleUrls: ['./movie-overview.component.scss']
})
export class MovieOverviewComponent implements OnInit, OnDestroy {
  generalStats$: Observable<GeneralMovieStatistics>;
  charts$: Observable<MovieCharts>;
  people$: Observable<PersonStatistics>;
  suspicious$: Observable<MovieSuspiciousContainer>;

  @ViewChild(NgScrollbar) textAreaScrollbar: NgScrollbar;

  selectedCollectionSub: Subscription;
  dropdownBlurredSub: Subscription;
  settingsSub: Subscription;
  isMovieTypePresentSub: Subscription;
  collections$: Observable<Collection[]>;
  collectionsFormControl = new FormControl('', { updateOn: 'blur' });
  typeIsPresent: boolean;

  settings: Settings;

  displayedColumns = ['position', 'title', 'reason', 'linkOne', 'qualityOne', 'addedOnOne', 'linkTwo', 'qualityTwo', 'addedOnTwo'];

  barOptions: Options;

  private selectedCollections: string[];

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

    this.generalStats$ = this.movieService.getGeneral([]);
    this.charts$ = this.movieService.getCharts([]);
    this.people$ = this.movieService.getPeople([]);
    this.suspicious$ = this.movieService.getSuspicious([]);

    this.collectionsFormControl.valueChanges.subscribe((collectionList: string[]) => {
      this.generalStats$ = this.movieService.getGeneral(collectionList);
      this.charts$ = this.movieService.getCharts(collectionList);
      this.people$ = this.movieService.getPeople(collectionList);
      this.suspicious$ = this.movieService.getSuspicious(collectionList);
    });
  }

  ngOnInit() {
  }

  @HostListener('window:resize', ['$event'])
  onResize(event) {
    this.generalStats$.subscribe(() => {
      this.textAreaScrollbar.update();
    });
  }

  openMovie(id: string): void {
    const embyUrl = ConfigHelper.getFullEmbyAddress(this.settings);
    window.open(`${embyUrl}/web/index.html#!/itemdetails.html?id=${id}`, '_blank');
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
