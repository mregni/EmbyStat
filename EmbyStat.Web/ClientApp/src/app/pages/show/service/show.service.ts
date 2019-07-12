import { Observable } from 'rxjs';
import { ShowStatistics } from 'src/app/shared/models/show/show-statistics';

import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

import { ListToQueryParam } from '../../../shared/helpers/list-to-query-param';
import { Collection } from '../../../shared/models/collection';
import { ShowCollectionRow } from '../../../shared/models/show/show-collection-row';

@Injectable()
export class ShowService {
  private readonly baseUrl = '/api/show';
  private getCollectionsUrl = this.baseUrl + '/collections';
  private getShowStatisticsUrl = this.baseUrl + '/statistics';
  private isTypePresentUrl = this.baseUrl + '/typepresent';
  private getCollectionRows = this.baseUrl + '/collectedlist';

  constructor(private http: HttpClient) {

  }

  getCollections(): Observable<Collection[]> {
    return this.http.get<Collection[]>(this.getCollectionsUrl);
  }

  getStatistics(list: string[]): Observable<ShowStatistics> {
    const params = ListToQueryParam.convert('collectionIds', list);
    return this.http.get<ShowStatistics>(this.getShowStatisticsUrl + params);
  }

  getCollectedList(list: string[]): Observable<ShowCollectionRow[]> {
    const params = ListToQueryParam.convert('collectionIds', list);
    return this.http.get<ShowCollectionRow[]>(this.getCollectionRows + params)
  }

  isTypePresent(): Observable<boolean> {
    return this.http.get<boolean>(this.isTypePresentUrl);
  }
}
