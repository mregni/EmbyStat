import { Component, OnInit, Input } from '@angular/core';
import { Observable } from 'rxjs/Observable';

import { MovieFacade } from '../state/facade.movie';
import { MovieStats } from '../models/movieStats';
import { LoaderFacade } from '../../shared/components/loader/state/facade.loader';

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
    if (collection === undefined) {
      collection = [];
    }

    this._selectedCollections = collection;
    this.stats$ = this.movieFacade.getGeneralStats(collection);
  }

  public stats$: Observable<MovieStats>;
  public isLoading$: Observable<boolean>;

  constructor(private movieFacade: MovieFacade, private loaderFacade: LoaderFacade) {

  }

  ngOnInit() {
    this.isLoading$ = this.loaderFacade.isMovieGeneralLoading();
  }

}
