import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { Store } from '@ngrx/store';
import 'rxjs/add/observable/throw';

import { Collection } from '../../shared/models/collection';
import { ShowStats } from '../models/showStats';
import { ShowGraphs } from '../models/showGraphs';
import { PersonStats } from '../../shared/models/personStats';
import { ShowCollectionRow } from '../models/showCollectionRow';

import { ShowService } from '../service/show.service';

import { ApplicationState } from '../../states/app.state';

@Injectable()
export class ShowFacade {
  constructor(private showService: ShowService ) { }

  getCollections(): Observable<Collection[]> {
    return this.showService.getCollections();
  }

  getGeneralStats(list: string[]): Observable<ShowStats> {
    return this.showService.getGeneralStats(list);
  }

  getGraphs(list: string[]): Observable<ShowGraphs> {
    return this.showService.getGraphs(list);
  }

  getPersonStats(list: string[]): Observable<PersonStats> {
    return this.showService.getPersonStats(list);
  }

  getCollectionList(list: string[]): Observable<ShowCollectionRow[]> {
    return this.showService.getCollectedList(list);
  }

  isShowTypePresent(): Observable<boolean> {
    return this.showService.checkIfTypeIsPresent();
  }
}
