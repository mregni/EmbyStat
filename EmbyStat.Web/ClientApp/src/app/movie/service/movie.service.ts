import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';

import { Observable } from 'rxjs/Observable';
import { MovieStats } from '../models/movieStats';
import { MoviePersonStats } from '../models/moviePersonStats';
import { Collection } from '../../shared/models/collection';
import { SuspiciousMovies } from '../models/suspiciousMovies';
import { MovieGraphs } from '../models/movieGraphs';

@Injectable()
export class MovieService {
  private readonly getGeneralUrl: string = '/movie/getgeneralstats';
  private readonly getPersonUrl: string = '/movie/getpersonstats';
  private readonly getCollectionsUrl: string = '/movie/getcollections';
  private readonly getSuspiciousUrl: string = '/movie/getsuspicious';
  private readonly getGraphsUrl: string = '/movie/getgraphs';

  constructor(private http: HttpClient) {

  }

  getGeneral(list: string[]): Observable<MovieStats> {
    return this.http.post<MovieStats>('/api' + this.getGeneralUrl, list);
  }

  getPerson(list: string[]): Observable<MoviePersonStats> {
    return this.http.post<MoviePersonStats>('/api' + this.getPersonUrl, list);
  }

  getCollections(): Observable<Collection[]> {
    return this.http.get<Collection[]>('/api' + this.getCollectionsUrl);
  }

  getSuspicious(list: string[]): Observable<SuspiciousMovies> {
    return this.http.post<SuspiciousMovies>('api' + this.getSuspiciousUrl, list);
  }

  getGraphs(list: string[]): Observable<MovieGraphs> {
    const params = new HttpParams();
    if (list.length > 0) {
      params.set('collectionIds', list.join(','));
    }
    const options = { params: params };
    return this.http.get<MovieGraphs>('api' + this.getGraphsUrl, options);
  }
}
