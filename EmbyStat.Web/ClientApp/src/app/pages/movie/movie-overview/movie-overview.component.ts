import { Observable, Subscription } from 'rxjs';
import { MovieCharts } from 'src/app/shared/models/movie/movie-charts';
import { MoviePeopleStatistics } from 'src/app/shared/models/movie/movie-people-statistics';
import { MovieSuspiciousContainer } from 'src/app/shared/models/movie/movie-suspicious-container';

import { Component, OnDestroy, OnInit } from '@angular/core';

import { CollectionService } from '../../../shared/behaviors/collection.service';
import { ConfigHelper } from '../../../shared/helpers/config-helper';
import { Collection } from '../../../shared/models/collection';
import { GeneralMovieStatistics } from '../../../shared/models/movie/general-movie-statistics';
import { Settings } from '../../../shared/models/settings/settings';
import { SettingsService } from '../../../shared/services/settings.service';
import { MovieService } from '../service/movie.service';

@Component({
  selector: 'app-movie-overview',
  templateUrl: './movie-overview.component.html',
  styleUrls: ['./movie-overview.component.scss']
})
export class MovieOverviewComponent implements OnInit, OnDestroy {
  generalStats$: Observable<GeneralMovieStatistics>;
  charts$: Observable<MovieCharts>;
  people$: Observable<MoviePeopleStatistics>;
  suspicious$: Observable<MovieSuspiciousContainer>;

  selectedCollectionSub: Subscription;
  dropdownBlurredSub: Subscription;
  settingsSub: Subscription;
  collectionSub: Subscription;

  settings: Settings;

  private selectedCollections: string[];

  constructor(
    private readonly settingsService: SettingsService,
    private readonly movieService: MovieService,
    private readonly collectionBehaviorService: CollectionService) {
    this.settingsSub = this.settingsService.getSettings().subscribe((settings: Settings) => {
      this.settings = settings;
    });

    this.collectionSub = this.movieService.getCollections().subscribe((list: Collection[]) => {
      this.collectionBehaviorService.setCollections(list);
      this.collectionBehaviorService.setPlaceholderSubject('MOVIES.COLLECTIONPLACEHOLDER');
      this.collectionBehaviorService.setVisibility(true);
    });

    this.generalStats$ = this.movieService.getGeneral([]);
    this.charts$ = this.movieService.getCharts([]);
    this.people$ = this.movieService.getPeople([]);
    this.suspicious$ = this.movieService.getSuspicious([]);

    this.selectedCollectionSub = this.collectionBehaviorService.selectedCollectionsSubject.subscribe((list: string[]) => {
      this.selectedCollections = list;
    });

    this.dropdownBlurredSub = this.collectionBehaviorService.dropdownBlurredSubject.subscribe((value: boolean) => {
      if (!!value) {
        this.generalStats$ = this.movieService.getGeneral(this.selectedCollections);
        this.charts$ = this.movieService.getCharts(this.selectedCollections);
        this.people$ = this.movieService.getPeople(this.selectedCollections);
        this.suspicious$ = this.movieService.getSuspicious(this.selectedCollections);
      }
    });
  }

  ngOnInit() {
  }

  openMovie(id: string): void {
    const embyUrl = ConfigHelper.getFullEmbyAddress(this.settings);
    window.open(`${embyUrl}/web/index.html#!/itemdetails.html?id=${id}`, '_blank');
  }

  ngOnDestroy() {
    this.collectionBehaviorService.resetDropdownValues();

    if (this.selectedCollectionSub !== undefined) {
      this.selectedCollectionSub.unsubscribe();
    }

    if (this.dropdownBlurredSub !== undefined) {
      this.dropdownBlurredSub.unsubscribe();
    }

    if (this.collectionSub !== undefined) {
      this.collectionSub.unsubscribe();
    }

    if (this.settingsSub !== undefined) {
      this.settingsSub.unsubscribe();
    }
  }
}
