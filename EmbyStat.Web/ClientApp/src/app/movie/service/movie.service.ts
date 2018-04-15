import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { Observable } from 'rxjs/Observable';
import { SmallStat } from "../../shared/models/smallStat";
import { Collection } from "../../shared/models/collection";

@Injectable()
export class MovieService {
  private readonly getGeneralUrl: string = '/movie/getgeneralstats';
  private readonly getCollectionsUrl: string = '/movie/getcollections';

  constructor(private http: HttpClient) {

  }

  getGeneral(list: string[]): Observable<SmallStat[]> {
    return this.http.post<SmallStat[]>('/api' + this.getGeneralUrl, list);
  }

  getCollections(): Observable<Collection[]> {
    return this.http.get<Collection[]>('/api' + this.getCollectionsUrl);
  }
}
