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

  isLoading(): Observable<boolean> {
    return this.store.select(LoaderQuery.isLoading);
  }
}
