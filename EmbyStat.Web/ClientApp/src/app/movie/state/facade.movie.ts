import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { Store } from '@ngrx/store';
import 'rxjs/add/observable/throw';

import { MovieStats } from '../models/movieStats';
import { PersonStats } from '../../shared/models/personStats';
import { Collection } from '../../shared/models/collection';
import { MovieGraphs } from '../models/movieGraphs';
import { SuspiciousMovies } from '../models/suspiciousMovies';

import { MovieService } from '../service/movie.service';

import { ApplicationState } from '../../states/app.state';

@Injectable()
export class MovieFacade {
  constructor(private movieService: MovieService) { }

  getGeneralStats(list: string[]): Observable<MovieStats> {
    return this.movieService.getGeneral(list);
  }

  getPeopleStats(list: string[]): Observable<PersonStats> {
    return this.movieService.getPerson(list);
  }

  getCollections(): Observable<Collection[]> {
    return this.movieService.getCollections();
  }

  getDuplicates(list: string[]): Observable<SuspiciousMovies> {
    return this.movieService.getSuspicious(list);
  }

  getGraphs(list: string[]): Observable<MovieGraphs> {
    return this.movieService.getGraphs(list);
  }

  isMovieTypePresent(): Observable<boolean> {
    return this.movieService.checkIfTypeIsPresent();
  }
}

