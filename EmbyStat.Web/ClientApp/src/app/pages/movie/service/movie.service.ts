import { Observable } from 'rxjs';

import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

import { ListToQueryParam } from '../../../shared/helpers/list-to-query-param';
import { Collection } from '../../../shared/models/collection';
import { MovieStatistics } from '../../../shared/models/movie/movie-statistics';

@Injectable()
export class MovieService {
  private readonly baseUrl = '/api/movie/';
  private getCollectionsUrl = this.baseUrl + 'collections';
  private getStatisticsUrl = this.baseUrl + 'statistics';
  private isTypePresentUrl = this.baseUrl + 'typepresent';

  constructor(private http: HttpClient) {

  }

  getCollections(): Observable<Collection[]> {
    return this.http.get<Collection[]>(this.getCollectionsUrl);
  }

  getStatistics(list: string[]): Observable<MovieStatistics> {
    const params = ListToQueryParam.convert('collectionIds', list);
    return this.http.get<MovieStatistics>(this.getStatisticsUrl + params);
  }

  isTypePresent(): Observable<boolean> {
    return this.http.get<boolean>(this.isTypePresentUrl);
  }
}
