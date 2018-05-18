import { Component, OnDestroy, Input } from '@angular/core';
import { MatTableDataSource } from '@angular/material';
import { Subscription } from 'rxjs/Subscription';

import { ShowCollectionRow } from '../models/showCollectionRow';
import { ShowFacade } from '../state/facade.show';

@Component({
  selector: 'app-show-collection',
  templateUrl: './show-collection.component.html',
  styleUrls: ['./show-collection.component.scss']
})
export class ShowCollectionComponent implements OnDestroy {
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
    this.dataSub = this.showFacade.getCollectionList(collection).subscribe(data => {
      this.dataSource.data = data;
    });

  }

  public displayedColumns = ['title', 'premiereDate', 'status', 'seasons', 'collected', 'percentage'];
  public dataSource = new MatTableDataSource();
  private dataSub: Subscription;
  constructor(private showFacade: ShowFacade) { }

  public getColor(collected: number, missing: number): string {
    const percentage = collected / (collected + missing) * 100;
    if (percentage === 100) {
      return '#5B990D';
    }
    else if (percentage >= 80) {
      return '#9DB269';
    }
    else if (percentage >= 60) {
      return '#F2A70D';
    }
    else if (percentage >= 40) {
      return '#F2700D';
    }
    else {
      return '#B11A10';
    }
  }

  ngOnDestroy(): void {
    if (this.dataSub !== undefined) {
      this.dataSub.unsubscribe();
    }
  }
}
