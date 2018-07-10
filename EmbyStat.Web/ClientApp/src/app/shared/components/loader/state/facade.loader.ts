import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { Store } from '@ngrx/store';
import 'rxjs/add/observable/throw';

import { LoaderQuery } from './reducer.loader';

import { ApplicationState } from '../../../../states/app.state';

@Injectable()
export class LoaderFacade {
  constructor(
    private store: Store<ApplicationState>
  ) { }

  isShowGeneralLoading(): Observable<boolean> {
    return this.store.select(LoaderQuery.isShowGeneralLoading);
  }

  isShowGraphsLoading(): Observable<boolean> {
    return this.store.select(LoaderQuery.isShowGraphsLoading);
  }

  isShowCollectionLoading(): Observable<boolean> {
    return this.store.select(LoaderQuery.isShowCollectionLoading);
  }

  isMovieGeneralLoading(): Observable<boolean> {
    return this.store.select(LoaderQuery.isMovieGeneralLoading);
  }

  isMovieGraphsLoading(): Observable<boolean> {
    return this.store.select(LoaderQuery.isMovieGraphsLoading);
  }

  isMoviePeopleLoading(): Observable<boolean> {
    return this.store.select(LoaderQuery.isMoviePeopleLoading);
  }

  isMovieSuspiciousLoading(): Observable<boolean> {
    return this.store.select(LoaderQuery.isMovieSuspiciousLoading);
  }
}
