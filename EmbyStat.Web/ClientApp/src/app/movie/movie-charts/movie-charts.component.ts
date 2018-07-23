import { Component, OnInit, Input, OnDestroy } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { Subscription } from 'rxjs/Subscription';

import { MovieChartsService } from '../service/movie-charts.service';
import { MovieFacade } from '../state/facade.movie';
import { MovieGraphs } from '../models/movieGraphs';
import { LoaderFacade } from '../../shared/components/loader/state/facade.loader';

@Component({
  selector: 'app-movie-charts',
  templateUrl: './movie-charts.component.html',
  styleUrls: ['./movie-charts.component.scss']
})
export class MovieChartsComponent implements OnInit, OnDestroy {
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

    this.movieFacade.clearGraphs();
    this.graphs$ = undefined;

    if (this.onTab) {
      this.graphs$ = this.movieFacade.getGraphs(this._selectedCollections);
    }
  }

  public graphs$: Observable<MovieGraphs>;
  public isLoading$: Observable<boolean>;

  private movieChartSub: Subscription;
  private onTab = false;

  constructor(private movieFacade: MovieFacade,
    private movieChartsService: MovieChartsService,
    private loaderFacade: LoaderFacade) {
    this.movieChartSub = movieChartsService.open.subscribe(value => {
      this.onTab = value;
      if (value && this.graphs$ === undefined) {
        this.graphs$ = this.movieFacade.getGraphs(this._selectedCollections);
      }
    });
  }

  ngOnInit() {
    this.isLoading$ = this.loaderFacade.isMovieGraphsLoading();
  }

  ngOnDestroy(): void {
    this.movieChartsService.changeOpened(false);

    if (this.movieChartSub !== undefined) {
      this.movieChartSub.unsubscribe();
    }
  }
}
