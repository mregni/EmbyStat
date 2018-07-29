import { Component, OnInit, Input } from '@angular/core';
import { Observable } from 'rxjs/Observable';

import { MovieFacade } from '../state/facade.movie';
import { PersonStats } from '../../shared/models/personStats';

@Component({
  selector: 'app-movie-people',
  templateUrl: './movie-people.component.html',
  styleUrls: ['./movie-people.component.scss']
})
export class MoviePeopleComponent implements OnInit {
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
    this.stats$ = this.movieFacade.getPeopleStats(collection);
  }

  public stats$: Observable<PersonStats>;
  
  constructor(private movieFacade: MovieFacade) {

  }

  ngOnInit() {

  }
}
