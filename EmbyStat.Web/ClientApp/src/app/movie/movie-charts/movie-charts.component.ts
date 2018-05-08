import { Component, OnInit, Input } from '@angular/core';
import { Observable } from 'rxjs/Observable';

import { MovieChartsService } from '../service/movie-charts.service';
import { MovieFacade } from '../state/facade.movie';
import { MovieGraphs } from '../models/movieGraphs';

@Component({
  selector: 'app-movie-charts',
  templateUrl: './movie-charts.component.html',
  styleUrls: ['./movie-charts.component.scss']
})
export class MovieChartsComponent implements OnInit  {
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
  }

  public graphs$: Observable<MovieGraphs>;

  constructor(private movieFacade: MovieFacade, private movieChartsService: MovieChartsService) {
    movieChartsService.open.subscribe(value => {
      if (value) {
        this.graphs$ = this.movieFacade.getGraphs(this._selectedCollections);
      }
    });
  }

  ngOnInit() {

  }
}
