import { Component, OnInit, Input } from '@angular/core';
import { Observable } from 'rxjs/Observable';

import { MovieFacade } from '../state/facade.movie';
import { MovieStats } from '../models/movieStats';

@Component({
  selector: 'app-movie-general',
  templateUrl: './movie-general.component.html',
  styleUrls: ['./movie-general.component.scss']
})
export class MovieGeneralComponent implements OnInit {
  private _selectedCollections: string[];

  get selectedCollections(): string[] {
    return this._selectedCollections;
  }

  @Input()
  set selectedCollections(collection: string[]) {
    console.log(collection);
    if (collection === undefined) {
      collection = [];
    }

    this._selectedCollections = collection;
    this.stats$ = this.movieFacade.getGeneralStats(collection);
  }

  public stats$: Observable<MovieStats>;

  constructor(private movieFacade: MovieFacade) {

  }

  ngOnInit() {
  }

}
