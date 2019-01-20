import { Component, OnInit, Input } from '@angular/core';
import { Observable } from 'rxjs/Observable';

import { ShowStats } from '../models/show-stats';
import { ShowService } from '../service/show.service';

@Component({
  selector: 'app-show-general',
  templateUrl: './show-general.component.html',
  styleUrls: ['./show-general.component.scss']
})

export class ShowGeneralComponent implements OnInit {
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
    this.stats$ = this.showService.getGeneralStats(collection);
  }

  stats$: Observable<ShowStats>;

  constructor(private showService: ShowService) { }

  ngOnInit() {

  }
}
