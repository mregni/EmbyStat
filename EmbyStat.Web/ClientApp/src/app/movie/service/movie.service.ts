import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ListToQueryParam } from '../../shared/helpers/listToQueryParam';

import { Observable } from 'rxjs/Observable';
import { MovieStats } from '../models/movieStats';
import { MoviePersonStats } from '../models/moviePersonStats';
import { Collection } from '../../shared/models/collection';
import { SuspiciousMovies } from '../models/suspiciousMovies';
import { MovieGraphs } from '../models/movieGraphs';

@Injectable()
export class MovieService {
  private readonly getGeneralUrl: string = '/movie/generalstats';
  private readonly getPersonUrl: string = '/movie/personstats';
  private readonly getCollectionsUrl: string = '/movie/collections';
  private readonly getSuspiciousUrl: string = '/movie/suspicious';
  private readonly getGraphsUrl: string = '/movie/getgraphs';

  constructor(private http: HttpClient) {

  }

  getGeneral(list: string[]): Observable<MovieStats> {
    const params = ListToQueryParam.convert('collectionIds', list);
    return this.http.get<MovieStats>('/api' + this.getGeneralUrl + params);
  }

  getPerson(list: string[]): Observable<MoviePersonStats> {
    const params = ListToQueryParam.convert('collectionIds', list);
    return this.http.get<MoviePersonStats>('/api' + this.getPersonUrl + params);
  }

  getCollections(): Observable<Collection[]> {
    return this.http.get<Collection[]>('/api' + this.getCollectionsUrl);
  }

  getSuspicious(list: string[]): Observable<SuspiciousMovies> {
    const params = ListToQueryParam.convert('collectionIds', list);
    return this.http.get<SuspiciousMovies>('api' + this.getSuspiciousUrl + params);
  }

  getGraphs(list: string[]): Observable<MovieGraphs> {
    const params = ListToQueryParam.convert('collectionIds', list);
    return this.http.get<MovieGraphs>('api' + this.getGraphsUrl + params);
  }
}

