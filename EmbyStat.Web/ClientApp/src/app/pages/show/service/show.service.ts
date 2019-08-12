import { Observable } from 'rxjs';
import { ShowStatistics } from 'src/app/shared/models/show/show-statistics';

import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

import { ListToQueryParam } from '../../../shared/helpers/list-to-query-param';
import { Library } from '../../../shared/models/library';
import { ShowCollectionRow } from '../../../shared/models/show/show-collection-row';

@Injectable()
export class ShowService {
  private readonly baseUrl = '/api/show';
  private getLibraryUrl = this.baseUrl + '/libraries';
  private getShowStatisticsUrl = this.baseUrl + '/statistics';
  private isTypePresentUrl = this.baseUrl + '/typepresent';
  private getCollectionRows = this.baseUrl + '/collectedlist';

  constructor(private http: HttpClient) {

  }

  getLibraries(): Observable<Library[]> {
    return this.http.get<Library[]>(this.getLibraryUrl);
  }

  getStatistics(list: string[]): Observable<ShowStatistics> {
    const params = ListToQueryParam.convert('libraryIds', list);
    return this.http.get<ShowStatistics>(this.getShowStatisticsUrl + params);
  }

  getCollectedList(list: string[]): Observable<ShowCollectionRow[]> {
    const params = ListToQueryParam.convert('libraryIds', list);
    return this.http.get<ShowCollectionRow[]>(this.getCollectionRows + params);
  }

  isTypePresent(): Observable<boolean> {
    return this.http.get<boolean>(this.isTypePresentUrl);
  }
}
