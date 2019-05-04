import { Observable } from 'rxjs';

import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

import { ListToQueryParam } from '../../../shared/helpers/list-to-query-param';
import { Collection } from '../../../shared/models/collection';
import { GeneralMovieStatistics } from '../../../shared/models/movie/general-movie-statistics';
import { MovieCharts } from '../../../shared/models/movie/movie-charts';
import { MoviePeopleStatistics } from '../../../shared/models/movie/movie-people-statistics';
import { MovieSuspiciousContainer } from '../../../shared/models/movie/movie-suspicious-container';

@Injectable()
export class MovieService {
  private readonly baseUrl = '/api/movie/';
  private getGeneralUrl = this.baseUrl + 'generalstats';
  private getCollectionsUrl = this.baseUrl + 'collections';
  private getChartsUrl = this.baseUrl + 'charts';
  private getPersonUrl = this.baseUrl + 'peoplestats';
  private getSuspiciousUrl = this.baseUrl + 'suspicious';

  constructor(private http: HttpClient) {

  }

  getGeneral(list: string[]): Observable<GeneralMovieStatistics> {
    const params = ListToQueryParam.convert('collectionIds', list);
    return this.http.get<GeneralMovieStatistics>(this.getGeneralUrl + params);
  }

  getCollections(): Observable<Collection[]> {
    return this.http.get<Collection[]>(this.getCollectionsUrl);
  }

  getCharts(list: string[]): Observable<MovieCharts> {
    const params = ListToQueryParam.convert('collectionIds', list);
    return this.http.get<MovieCharts>(this.getChartsUrl + params);
  }

  getPeople(list: string[]): Observable<MoviePeopleStatistics> {
    const params = ListToQueryParam.convert('collectionIds', list);
    return this.http.get<MoviePeopleStatistics>(this.getPersonUrl + params);
  }

  getSuspicious(list: string[]): Observable<MovieSuspiciousContainer> {
    const params = ListToQueryParam.convert('collectionIds', list);
    return this.http.get<MovieSuspiciousContainer>(this.getSuspiciousUrl + params);
  }
}
