import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ListToQueryParam } from '../../shared/helpers/listToQueryParam';

import { Observable } from 'rxjs';
import { Collection } from '../../shared/models/collection';
import { ShowStats } from '../models/show-stats';
import { ShowGraphs } from '../models/show-graphs';
import { PersonStats } from '../../shared/models/person-stats';
import { ShowCollectionRow } from '../models/show-collection-row';

@Injectable()
export class ShowService {
  private readonly baseUrl = '/api/show';
  private getCollectionsUrl = this.baseUrl + '/collections';
  private getGeneralStatsUrl = this.baseUrl + '/generalstats';
  private getGraphsUrl = this.baseUrl + '/graphs';
  private getPersonStatsUrl = this.baseUrl + '/personstats';
  private getCollectedListUrl = this.baseUrl + '/collectedlist';
  private checkIfTypeIsPresentUrl = this.baseUrl + '/showtypepresent';


  constructor(private http: HttpClient) {

  }

  getCollections(): Observable<Collection[]> {
    return this.http.get<Collection[]>(this.getCollectionsUrl);
  }

  getGeneralStats(list: string[]): Observable<ShowStats> {
    const params = ListToQueryParam.convert('collectionIds', list);
    return this.http.get<ShowStats>(this.getGeneralStatsUrl + params);
  }

  getGraphs(list: string[]): Observable<ShowGraphs> {
    const params = ListToQueryParam.convert('collectionIds', list);
    return this.http.get<ShowGraphs>(this.getGraphsUrl + params);
  }

  getPersonStats(list: string[]): Observable<PersonStats> {
    const params = ListToQueryParam.convert('collectionIds', list);
    return this.http.get<PersonStats>(this.getPersonStatsUrl + params);
  }

  getCollectedList(list: string[]): Observable<ShowCollectionRow[]> {
    const params = ListToQueryParam.convert('collectionIds', list);
    return this.http.get<ShowCollectionRow[]>(this.getCollectedListUrl + params);
  }

  checkIfTypeIsPresent(): Observable<boolean> {
    return this.http.get<boolean>(this.checkIfTypeIsPresentUrl);
  }
}
