import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ListToQueryParam } from '../../shared/helpers/listToQueryParam';

import { Observable } from 'rxjs';
import { MovieStats } from '../models/movie-stats';
import { PersonStats } from '../../shared/models/person-stats';
import { Collection } from '../../shared/models/collection';
import { SuspiciousMovieContainer } from '../models/suspicious-movie-container';
import { MovieGraphs } from '../models/movie-graphs';

@Injectable()
export class MovieService {
  private readonly baseUrl = '/api/movie/';
  private getGeneralUrl = this.baseUrl + 'generalstats';
  private getPersonUrl = this.baseUrl + 'personstats';
  private getCollectionsUrl = this.baseUrl + 'collections';
  private getSuspiciousUrl = this.baseUrl + 'suspicious';
  private getGraphsUrl = this.baseUrl + 'graphs';
  private checkIfTypeIsPresentUrl = this.baseUrl + 'movietypepresent';

  constructor(private http: HttpClient) {

  }

  getGeneral(list: string[]): Observable<MovieStats> {
    const params = ListToQueryParam.convert('collectionIds', list);
    return this.http.get<MovieStats>(this.getGeneralUrl + params);
  }

  getPeople(list: string[]): Observable<PersonStats> {
    const params = ListToQueryParam.convert('collectionIds', list);
    return this.http.get<PersonStats>(this.getPersonUrl + params);
  }

  getCollections(): Observable<Collection[]> {
    return this.http.get<Collection[]>(this.getCollectionsUrl);
  }

  getSuspicious(list: string[]): Observable<SuspiciousMovieContainer> {
    const params = ListToQueryParam.convert('collectionIds', list);
    return this.http.get<SuspiciousMovieContainer>(this.getSuspiciousUrl + params);
  }

  getGraphs(list: string[]): Observable<MovieGraphs> {
    const params = ListToQueryParam.convert('collectionIds', list);
    return this.http.get<MovieGraphs>(this.getGraphsUrl + params);
  }

  checkIfTypeIsPresent(): Observable<boolean> {
    return this.http.get<boolean>(this.checkIfTypeIsPresentUrl);
  }
}
