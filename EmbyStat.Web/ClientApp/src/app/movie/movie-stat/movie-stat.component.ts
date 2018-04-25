import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { FormControl } from '@angular/forms';

import { MovieFacade } from '../state/facade.movie';
import { Collection } from '../../shared/models/collection';
import { MovieStats } from '../models/movieStats';


@Component({
  selector: 'app-movie-stat',
  templateUrl: './movie-stat.component.html',
  styleUrls: ['./movie-stat.component.scss']
})
export class MovieStatComponent implements OnInit {
  public collections$: Observable<Collection[]>;
  public selectedCollections: string[];
  
  public collectionsFormControl = new FormControl('', { updateOn: 'blur' });

  constructor(private movieFacade: MovieFacade) {
    this.collections$ = this.movieFacade.getCollections();

    this.collectionsFormControl.valueChanges.subscribe(data => {
      this.selectedCollections = data;
    });
  }

  ngOnInit() {
    
  }

}
