import { Component, OnInit, Input } from '@angular/core';
import { Observable } from 'rxjs';

import { MovieService } from '../service/movie.service';
import { MovieStats } from '../models/movie-stats';

@Component({
  selector: 'app-movie-general',
  templateUrl: './movie-general.component.html',
  styleUrls: ['./movie-general.component.scss']
})
export class MovieGeneralComponent implements OnInit {
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
    this.stats$ = this.movieService.getGeneral(collection);
  }

  stats$: Observable<MovieStats>;

  constructor(private movieService: MovieService) {

  }

  ngOnInit() {
  }

}
