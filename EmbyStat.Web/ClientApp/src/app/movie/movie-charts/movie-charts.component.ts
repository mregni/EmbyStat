import { Component, OnInit, Input, OnDestroy } from '@angular/core';
import { Observable ,  Subscription } from 'rxjs';

import { MovieChartsService } from '../service/movie-charts.service';
import { MovieService } from '../service/movie.service';
import { MovieGraphs } from '../models/movie-graphs';

@Component({
  selector: 'app-movie-charts',
  templateUrl: './movie-charts.component.html',
  styleUrls: ['./movie-charts.component.scss']
})
export class MovieChartsComponent implements OnInit, OnDestroy {
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
    this.graphs$ = undefined;

    if (this.onTab) {
      this.graphs$ = this.movieService.getGraphs(this.selectedCollectionsPriv);
    }
  }

  public graphs$: Observable<MovieGraphs>;

  private movieChartSub: Subscription;
  private onTab = false;

  constructor(private movieService: MovieService, private movieChartsService: MovieChartsService) {
    this.movieChartSub = movieChartsService.open.subscribe(value => {
      this.onTab = value;
      if (value && this.graphs$ === undefined) {
        this.graphs$ = this.movieService.getGraphs(this.selectedCollectionsPriv);
      }
    });
  }

  ngOnInit() {
  }

  ngOnDestroy(): void {
    this.movieChartsService.changeOpened(false);

    if (this.movieChartSub !== undefined) {
      this.movieChartSub.unsubscribe();
    }
  }
}
