import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription ,  Observable } from 'rxjs';
import { FormControl } from '@angular/forms';
import { MatDialog } from '@angular/material';
import { ActivatedRoute } from '@angular/router';

import { NoTypeFoundDialog } from '../../shared/dialogs/no-type-found/no-type-found.component';
import { MovieChartsService } from '../service/movie-charts.service';
import { MovieService } from '../service/movie.service';

import { Collection } from '../../shared/models/collection';

@Component({
  selector: 'app-movie-overview',
  templateUrl: './movie-overview.component.html',
  styleUrls: ['./movie-overview.component.scss']
})
export class MovieOverviewComponent implements OnInit, OnDestroy {
  private isMovieTypePresentSub: Subscription;
  private paramSub: Subscription;

  selected = 0;
  collections$: Observable<Collection[]>;
  selectedCollections: string[];
  collectionsFormControl = new FormControl('', { updateOn: 'blur' });
  typeIsPresent: boolean;

  constructor(
    private movieService: MovieService,
    private movieChartsService: MovieChartsService,
    public dialog: MatDialog,
    private activatedRoute: ActivatedRoute) {
    this.collections$ = this.movieService.getCollections();
    this.isMovieTypePresentSub = this.movieService.checkIfTypeIsPresent().subscribe((typePresent: boolean) => {
      this.typeIsPresent = typePresent;
      if (!typePresent) {
        this.dialog.open(NoTypeFoundDialog,
          {
            width: '550px',
            data: 'MOVIES'
          });
      }

      this.paramSub = this.activatedRoute.params.subscribe(params => {
        const tab = params['tab'];
        switch (tab) {
          case "charts":
            this.selected = 1;
            break;
          case "people":
            this.selected = 2;
            break;
          case "suspicious":
            this.selected = 3;
            break;
          default:
            this.selected = 0;
        }
      });
    });

    this.collectionsFormControl.valueChanges.subscribe(data => {
      this.selectedCollections = data;
    });

  }

  ngOnInit() { }

  ngOnDestroy(): void {
    if (this.isMovieTypePresentSub !== undefined) {
      this.isMovieTypePresentSub.unsubscribe();
    }

    if (this.paramSub !== undefined) {
      this.paramSub.unsubscribe();
    }
  }

  onTabChanged(event): void {
    if (event.index === 1) {
      this.movieChartsService.changeOpened(true);
    } else {
      this.movieChartsService.changeOpened(false);
    }
  }
}
