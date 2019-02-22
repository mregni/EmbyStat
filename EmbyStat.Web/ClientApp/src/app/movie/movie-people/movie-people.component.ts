import { Component, OnInit, Input } from '@angular/core';
import { Observable } from 'rxjs';

import { MovieService } from '../service/movie.service';
import { PersonStats } from '../../shared/models/person-stats';

@Component({
  selector: 'app-movie-people',
  templateUrl: './movie-people.component.html',
  styleUrls: ['./movie-people.component.scss']
})
export class MoviePeopleComponent implements OnInit {
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
    this.stats$ = this.movieService.getPeople(collection);
  }

  stats$: Observable<PersonStats>;

  constructor(private movieService: MovieService) {

  }

  ngOnInit() {

  }
}
