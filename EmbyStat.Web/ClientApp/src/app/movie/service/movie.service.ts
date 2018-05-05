import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { Observable } from 'rxjs/Observable';
import { MovieStats } from '../models/movieStats';
import { MoviePersonStats } from '../models/moviePersonStats';
import { Collection } from "../../shared/models/collection";
import { Duplicate } from "../models/graphs/duplicate";

@Injectable()
export class MovieService {
  private readonly getGeneralUrl: string = '/movie/getgeneralstats';
  private readonly getPersonUrl: string = '/movie/getpersonstats';
  private readonly getCollectionsUrl: string = '/movie/getcollections';
  private readonly getDuplicateUrl: string = '/movie/getduplicates';

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

  getDuplicateGraph(list: string[]): Observable<Duplicate[]> {
    return this.http.post<Duplicate[]>('api' + this.getDuplicateUrl, list);
  }
}
