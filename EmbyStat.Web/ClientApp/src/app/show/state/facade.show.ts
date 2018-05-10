import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { Store } from '@ngrx/store';
import 'rxjs/add/observable/throw';

import { Collection } from '../../shared/models/collection';

import { ShowQuery } from './reducer.show';
import {
  LoadShowCollectionsAction
  } from './actions.show';

import { ApplicationState } from '../../states/app.state';

@Injectable()
export class ShowFacade {
  constructor(
    private store: Store<ApplicationState>
  ) { }

  collections$ = this.store.select(ShowQuery.getCollections);

  getCollections(): Observable<Collection[]> {
    this.store.dispatch(new LoadShowCollectionsAction());
    return this.collections$;
  }
}
