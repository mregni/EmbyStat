import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs/Subscription';
import { Observable } from 'rxjs/Observable';
import { FormControl } from '@angular/forms';
import { MatDialog } from '@angular/material';

import { NoTypeFoundDialog } from '../../shared/dialogs/no-type-found/no-type-found.component';
import { MovieChartsService } from '../service/movie-charts.service';
import { MovieFacade } from '../state/facade.movie';

import { Collection } from '../../shared/models/collection';

@Component({
  selector: 'app-movie-overview',
  templateUrl: './movie-overview.component.html',
  styleUrls: ['./movie-overview.component.scss']
})
export class MovieOverviewComponent implements OnInit, OnDestroy {
  public collections$: Observable<Collection[]>;
  public selectedCollections: string[];
  private isMovieTypePresentSub: Subscription;

  public collectionsFormControl = new FormControl('', { updateOn: 'blur' });
  public typeIsPresent: boolean;

  constructor(
    private movieFacade: MovieFacade,
    private movieChartsService: MovieChartsService,
    public dialog: MatDialog) {
    this.collections$ = this.movieFacade.getCollections();
    this.isMovieTypePresentSub = this.movieFacade.isMovieTypePresent().subscribe((typePresent: boolean) => {
      this.typeIsPresent = typePresent;
      if (!typePresent) {
        this.dialog.open(NoTypeFoundDialog,
          {
            width: '550px',
            data: 'MOVIES'
          });
      }
    });

    this.collectionsFormControl.valueChanges.subscribe(data => {
      this.selectedCollections = data;
    });

  }

  ngOnInit() {
  
  }

  ngOnDestroy(): void {
    if (this.isMovieTypePresentSub !== undefined) {
      this.isMovieTypePresentSub.unsubscribe();
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
