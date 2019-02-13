import { Component, OnDestroy, OnInit, Input } from '@angular/core';
import { MatTableDataSource } from '@angular/material';
import { Subscription } from 'rxjs';
import { SettingsFacade } from '../../settings/state/facade.settings';
import { Settings } from '../../settings/models/settings';
import { ConfigHelper } from '../../shared/helpers/configHelper';

import { MovieService } from '../service/movie.service';

@Component({
  selector: 'app-movie-suspicious',
  templateUrl: './movie-suspicious.component.html',
  styleUrls: ['./movie-suspicious.component.scss']
})
export class MovieSuspiciousComponent implements OnInit, OnDestroy {
  public suspiciousDisplayedColumns = ['number', 'title', 'reason', 'linkOne', 'qualityOne',
    'dateCreatedOne', 'linkTwo', 'qualityTwo', 'dateCreatedTwo'];
  suspiciousDataSource = new MatTableDataSource();

  shortDisplayedColumns = ['number', 'title', 'duration', 'link' ];
  shortDataSource = new MatTableDataSource();

  noImdbDisplayedColumns = ['number', 'title', 'link'];
  noImdbDataSource = new MatTableDataSource();

  noPrimaryDisplayedColumns = ['number', 'title', 'link'];
  noPrimaryDataSource = new MatTableDataSource();

  private duplicatesSub: Subscription;
  private settingsSub: Subscription;
  private settings: Settings;

  private selectedCollectionsPriv: string[];
  get selectedCollections(): string[] {
    return this.selectedCollectionsPriv;
  }

  @Input()
  set selectedCollections(collection: string[]) {
    if (collection === undefined) {
      collection = [];
    }

    this.selectedCollectionsPriv = collection;
    this.duplicatesSub = this.movieService.getSuspicious(collection).subscribe(data => {
      this.suspiciousDataSource.data = data.duplicates;
      this.shortDataSource.data = data.shorts;
      this.noImdbDataSource.data = data.noImdb;
      this.noPrimaryDataSource.data = data.noPrimary;
    });
  }

  constructor(
    private readonly movieService: MovieService,
    private readonly settingsFacade: SettingsFacade) {
    this.settingsSub = settingsFacade.getSettings().subscribe((settings: Settings) => this.settings = (settings));
  }

  ngOnInit() {

  }

  openMovie(id: string): void {
    const embyUrl = ConfigHelper.getFullEmbyAddress(this.settings);
    window.open(`${embyUrl}/web/index.html#!/itemdetails.html?id=${id}`, '_blank');
  }

  ngOnDestroy(): void {
    if (this.duplicatesSub !== undefined) {
      this.duplicatesSub.unsubscribe();
    }

    if (this.settingsSub !== undefined) {
      this.settingsSub.unsubscribe();
    }
  }
}
