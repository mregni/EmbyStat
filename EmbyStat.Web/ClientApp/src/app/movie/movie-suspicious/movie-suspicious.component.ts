import { Component, OnDestroy, Input } from '@angular/core';
import { MatTableDataSource } from '@angular/material';
import { Subscription } from 'rxjs/Subscription';
import { ConfigurationFacade } from '../../configuration/state/facade.configuration';
import { Configuration } from '../../configuration/models/configuration';

import { MovieFacade } from '../state/facade.movie';

@Component({
  selector: 'app-movie-suspicious',
  templateUrl: './movie-suspicious.component.html',
  styleUrls: ['./movie-suspicious.component.scss']
})
export class MovieSuspiciousComponent implements OnDestroy {
  public displayedColumns: string[] = [ 'number', 'title', 'reason', 'linkOne', 'qualityOne', 'dateCreatedOne', 'linkTwo', 'qualityTwo', 'dateCreatedTwo'];
  public dataSource = new MatTableDataSource();
  private duplicatesSub: Subscription;
  private configurationSub: Subscription;
  private configuration: Configuration;

  private _selectedCollections: string[];
  get selectedCollections(): string[] {
    return this._selectedCollections;
  }

  @Input()
  set selectedCollections(collection: string[]) {
    if (collection === undefined) {
      collection = [];
    }

    this._selectedCollections = collection;
    this.duplicatesSub = this.movieFacade.getDuplicates(collection).subscribe(data => this.dataSource.data = data);
  }


  constructor(private movieFacade: MovieFacade, private configurationFacade: ConfigurationFacade) {
    this.configurationSub = configurationFacade.getConfiguration().subscribe(data => this.configuration = data);
  }

  openMovie(id: string): void {
    window.open(`${this.configuration.embyServerAddress}/emby/web/itemdetails.html?id=${id}`, '_blank');
  }

  ngOnDestroy(): void {
    if (this.duplicatesSub !== undefined) {
      this.duplicatesSub.unsubscribe();
    }
  }

}
