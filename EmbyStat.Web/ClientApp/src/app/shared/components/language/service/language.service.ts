import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { Language } from '../models/language';

@Injectable()
export class LanguageService {
  private readonly getLangaugesUrl: string = '/language/getlist';

  constructor(private http: HttpClient) { }

  getLanguages(): Observable<Language[]> {
    return this.http.get<Language[]>('/api' + this.getLangaugesUrl);
  }
}
