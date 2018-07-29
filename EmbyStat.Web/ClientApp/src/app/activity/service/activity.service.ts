import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { Observable } from 'rxjs/Observable';

import { Activity } from '../models/activity';

@Injectable()
export class ActivityService {
  private readonly getActivityUrl: string = '/activity';

  constructor(private http: HttpClient) {

  }

  getActivities(): Observable<Activity[]> {
    return this.http.get<Activity>('/api' + this.getActivityUrl);
  }
}
