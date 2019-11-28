import { Observable } from 'rxjs';

import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';

import { Job } from '../models/jobs/job';

@Injectable({
  providedIn: 'root'
})
export class JobService {
  private readonly baseUrl = '/api/job';
  private fireUrl = `${this.baseUrl}/fire/`;
  private getMediaSyncJobUrl = `${this.baseUrl}/mediasync`;

  constructor(private http: HttpClient) {

  }

  getAll(): Observable<Job[]> {
    return this.http.get<Job[]>(this.baseUrl);
  }

  getById(id: string): Observable<Job> {
    return this.http.get<Job>(this.baseUrl + '/' + id);
  }

  updateTrigger(id: string, cron: string): Observable<void> {
    const httpParams = new HttpParams().append('cron', cron);
    return this.http.patch<void>(this.baseUrl + '/' + id, {}, { params: httpParams });
  }

  fireJob(id: string): Observable<void> {
    return this.http.post<void>(this.fireUrl + id, {});
  }

  getMediaSyncJob(): Observable<Job> {
    return this.http.get<Job>(this.getMediaSyncJobUrl);
  }
}
