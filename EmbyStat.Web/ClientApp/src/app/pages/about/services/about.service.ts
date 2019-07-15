import { Observable } from 'rxjs';

import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

import { About } from '../models/about';

@Injectable({
  providedIn: 'root'
})
export class AboutService {
  private readonly baseUrl = '/api/about';
  constructor(private readonly http: HttpClient) {

  }

  getAbout(): Observable<About> {
    return this.http.get<About>(this.baseUrl);
  }
}
