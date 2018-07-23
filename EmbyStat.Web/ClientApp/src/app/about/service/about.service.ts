import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { Observable } from 'rxjs/Observable';

import { About } from '../models/about';

@Injectable()
export class AboutService {
  private readonly getAboutUrl: string = '/about';

  constructor(private http: HttpClient) {

  }

  getAbout(): Observable<About> {
    return this.http.get<About>('/api' + this.getAboutUrl);
  }
}
