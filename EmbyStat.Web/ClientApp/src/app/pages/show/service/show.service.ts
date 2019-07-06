import { Observable } from 'rxjs';

import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

import { ListToQueryParam } from '../../../shared/helpers/list-to-query-param';
import { Collection } from '../../../shared/models/collection';
import { PersonStatistics } from '../../../shared/models/common/person-statistics';
import { GeneralShowStatistics } from '../../../shared/models/show/general-show-statistics';
import { ShowCharts } from '../../../shared/models/show/show-charts';
import { ShowCollectionRow } from '../../../shared/models/show/show-collection-row';

@Injectable()
export class ShowService {
  private readonly baseUrl = '/api/show';
  private getCollectionsUrl = this.baseUrl + '/collections';
  private getGeneralStatsUrl = this.baseUrl + '/generalstats';
  private getChartsUrl = this.baseUrl + '/charts';
  private getPersonStatsUrl = this.baseUrl + '/personstats';
  private getCollectedListUrl = this.baseUrl + '/collectedlist';
  private isTypePresentUrl = this.baseUrl + '/typepresent';

  constructor(private http: HttpClient) {

  }

  getCollections(): Observable<Collection[]> {
    return this.http.get<Collection[]>(this.getCollectionsUrl);
  }

  getGeneralStats(list: string[]): Observable<GeneralShowStatistics> {
    const params = ListToQueryParam.convert('collectionIds', list);
    return this.http.get<GeneralShowStatistics>(this.getGeneralStatsUrl + params);
  }

  getCharts(list: string[]): Observable<ShowCharts> {
    const params = ListToQueryParam.convert('collectionIds', list);
    return this.http.get<ShowCharts>(this.getChartsUrl + params);
  }

  getPersonStats(list: string[]): Observable<PersonStatistics> {
    const params = ListToQueryParam.convert('collectionIds', list);
    return this.http.get<PersonStatistics>(this.getPersonStatsUrl + params);
  }

  getCollectedList(list: string[]): Observable<ShowCollectionRow[]> {
    const params = ListToQueryParam.convert('collectionIds', list);
    return this.http.get<ShowCollectionRow[]>(this.getCollectedListUrl + params);
  }

  isTypePresent(): Observable<boolean> {
    return this.http.get<boolean>(this.isTypePresentUrl);
  }
}
