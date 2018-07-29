import { Component, OnDestroy, OnInit, Input } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { Subscription } from 'rxjs/Subscription';
import * as _ from 'lodash';

import { ShowCollectionRow } from '../models/showCollectionRow';
import { ShowFacade } from '../state/facade.show';

@Component({
  selector: 'app-show-collection',
  templateUrl: './show-collection.component.html',
  styleUrls: ['./show-collection.component.scss']
})
export class ShowCollectionComponent implements OnInit, OnDestroy {
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
    this.rowsSub = this.showFacade.getCollectionList(collection).subscribe(data => {
      this.rows = data;
    });
  }

  public rows: ShowCollectionRow[];

  private rowsSub: Subscription;
  private sortNameAsc = false;
  private sortStatusAsc = false;
  private sortSeasonsAsc = false;
  private sortEpisodesAsc = false;
  private sortpercentageAsc = false;
  private sortDateAsc = false;

  constructor(private showFacade: ShowFacade) { }

  ngOnInit() {
  
  }

  public getColor(row: ShowCollectionRow): string {
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

  public order(column: string): void {
    if (column === 'sortName') {
      this.rows = _.orderBy(this.rows, ['sortName'], [this.boolToSortString(this.sortNameAsc)]);
      this.sortNameAsc = !this.sortNameAsc;
    } else if (column === 'status') {
      this.rows = _.orderBy(this.rows, ['status', 'sortName'],
        [this.boolToSortString(this.sortStatusAsc), this.boolToSortString(this.sortStatusAsc)]);
      this.sortStatusAsc = !this.sortStatusAsc;
    } else if (column === 'seasons') {
      this.rows = _.orderBy(this.rows, ['seasons', 'sortName'],
        [this.boolToSortString(this.sortSeasonsAsc), this.boolToSortString(this.sortSeasonsAsc)]);
      this.sortSeasonsAsc = !this.sortSeasonsAsc;
    } else if (column === 'episodes') {
      this.rows = _.orderBy(this.rows, ['episodes', 'sortName'],
        [this.boolToSortString(this.sortEpisodesAsc), this.boolToSortString(this.sortEpisodesAsc)]);
      this.sortEpisodesAsc = !this.sortEpisodesAsc;
    } else if (column === 'percentage') {
      this.rows = _.orderBy(this.rows, [d => this.calculatePercentage(d), 'sortName'],
        [this.boolToSortString(this.sortpercentageAsc), this.boolToSortString(this.sortpercentageAsc)]);
      this.sortpercentageAsc = !this.sortpercentageAsc;
    } else if (column === 'date') {
      this.rows = _.orderBy(this.rows, ['premiereDate', 'sortName'],
        [this.boolToSortString(this.sortDateAsc), this.boolToSortString(this.sortDateAsc)]);
      this.sortDateAsc = !this.sortDateAsc;
    }
  }

  public calculatePercentage(row: ShowCollectionRow): number {
    if (row.episodes + row.missingEpisodes === 0) {
      return 0;
    } else {
      return row.episodes / (row.episodes + row.missingEpisodes);
    }
  }

  private boolToSortString(value: boolean): string {
    return value ? 'asc' : 'desc';
  }

  ngOnDestroy(): void {
    if (this.rowsSub !== undefined) {
      this.rowsSub.unsubscribe();
    }
  }
}
