import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { Observable } from 'rxjs/Observable';
import { SmallStat } from "../../shared/models/smallStat";
@Injectable()
export class MovieService {
  private readonly getGeneralUrl: string = '/movie/general';
  private readonly fireTaskUrl: string = '/task/fire';
  private readonly triggersUrl: string = '/task/triggers';

  constructor(private http: HttpClient) {

  }

  getGeneral(): Observable<SmallStat[]> {
    return this.http.get<SmallStat[]>('/api' + this.getGeneralUrl);
  }
}
