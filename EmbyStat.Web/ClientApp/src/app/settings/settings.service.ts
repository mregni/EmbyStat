import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { Observable } from 'rxjs';

import { Settings } from './models/settings';

@Injectable()
export class SettingsService {
  private readonly baseUrl = '/api/settings';

  constructor(private http: HttpClient) {

  }

  getSettings(): Observable<Settings> {
    return this.http.get<Settings>(this.baseUrl);
  }

  updateSettings(settings: Settings): Observable<Settings> {
    return this.http.put<Settings>(this.baseUrl, settings);
  }
}
