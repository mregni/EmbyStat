import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ListToQueryParam } from '../../shared/helpers/listToQueryParam';

import { Observable } from 'rxjs/Observable';
import { Collection } from '../../shared/models/collection';
import { ShowStats } from '../models/showStats';
import { ShowGraphs } from '../models/showGraphs';
import { PersonStats } from '../../shared/models/personStats';

@Injectable()
export class ShowService {
  private readonly getCollectionsUrl: string = '/show/collections';
  private readonly getGeneralStatsUrl: string = '/show/generalstats';
  private readonly getGraphsUrl: string = '/show/graphs';
  private readonly getPersonStatsUrl: string = '/show/personstats';
  constructor(private http: HttpClient) {

  }

  getCollections(): Observable<Collection[]> {
    return this.http.get<Collection[]>('/api' + this.getCollectionsUrl);
  }

  getGeneralStats(list: string[]): Observable<ShowStats> {
    const params = ListToQueryParam.convert('collectionIds', list);
    return this.http.get<ShowStats>('/api' + this.getGeneralStatsUrl + params);
  }

  getGraphs(list: string[]): Observable<ShowGraphs> {
    const params = ListToQueryParam.convert('collectionIds', list);
    return this.http.get<ShowGraphs>('/api' + this.getGraphsUrl + params);
  }

  getPersonStats(list: string[]): Observable<PersonStats> {
    const params = ListToQueryParam.convert('collectionIds', list);
    return this.http.get<PersonStats>('/api' + this.getPersonStatsUrl + params);
  }
}
