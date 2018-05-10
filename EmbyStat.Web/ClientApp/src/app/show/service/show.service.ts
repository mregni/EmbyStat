import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';

import { Observable } from 'rxjs/Observable';
import { Collection } from '../../shared/models/collection';

@Injectable()
export class ShowService {
  private readonly getCollectionsUrl: string = '/show/collections';
  constructor(private http: HttpClient) {

  }

  getCollections(): Observable<Collection[]> {
    return this.http.get<Collection[]>('/api' + this.getCollectionsUrl);
  }
}
