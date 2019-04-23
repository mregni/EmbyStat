import { Observable } from 'rxjs';

import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

import { Settings } from '../models/settings/settings';

@Injectable()
export class SettingsService {
  private readonly baseUrl = '/api/settings';

  constructor(private http: HttpClient) {

  }

  getSettings(): Observable<Settings> {
    return this.http.get<Settings>(this.baseUrl);
  }
}
