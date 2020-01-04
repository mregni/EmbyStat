import { Observable } from 'rxjs';

import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

import { ListToQueryParam } from '../../../shared/helpers/list-to-query-param';
import { Library } from '../../../shared/models/library';
import { MovieStatistics } from '../../../shared/models/movie/movie-statistics';

@Injectable()
export class MovieService {
  private readonly baseUrl = '/api/movie/';
    private getLibrariesUrl = `${this.baseUrl}libraries`;
    private getStatisticsUrl = `${this.baseUrl}statistics`;
    private isTypePresentUrl = `${this.baseUrl}typepresent`;

  constructor(private http: HttpClient) {

  }

  getLibraries(): Observable<Library[]> {
    return this.http.get<Library[]>(this.getLibrariesUrl);
  }

  getStatistics(list: string[]): Observable<MovieStatistics> {
    const params = ListToQueryParam.convert('libraryIds', list);
    return this.http.get<MovieStatistics>(this.getStatisticsUrl + `${params}`);
  }

  isTypePresent(): Observable<boolean> {
    return this.http.get<boolean>(this.isTypePresentUrl);
  }
}
