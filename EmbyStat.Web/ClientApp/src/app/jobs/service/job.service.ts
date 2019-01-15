import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';

import { Observable } from 'rxjs/Observable';
import { Job } from '../models/job';

@Injectable()
export class JobService {
  private readonly baseUrl = '/api/job';
  private firePingJobUrl = this.baseUrl + '/ping/fire';
  private fireCheckUpdateJobUrl = this.baseUrl + '/checkupdate/fire';
  private fireMediaSyncJobUrl = this.baseUrl + '/mediasync/fire';
  private fireSmallSyncUrl = this.baseUrl + '/smallsync/fire';
  private fireDatabaseCleanupUrl = this.baseUrl + '/databasecleanup/fire';

  constructor(private http: HttpClient) {

  }

  getAll(): Observable<Job[]> {
    return this.http.get<Job[]>(this.baseUrl);
  }

  getById(id: string): Observable<Job> {
    return this.http.get<Job>(this.baseUrl + '/' + id);
  }

  firePingJob(): Observable<void> {
    return this.http.post<void>(this.firePingJobUrl, null);
  }

  fireCheckUpdateJob(): Observable<void> {
    return this.http.post<void>(this.fireCheckUpdateJobUrl, null);
  }

  fireMediaSyncJob(): Observable<void> {
    return this.http.post<void>(this.fireMediaSyncJobUrl, null);
  }

  fireSmallSyncJob(): Observable<void> {
    return this.http.post<void>(this.fireSmallSyncUrl, null);
  }

  fireDatabaseCleanupJob(): Observable<void> {
    return this.http.post<void>(this.fireDatabaseCleanupUrl, null);
  }

  updateTrigger(id: string, cron: string): Observable<void> {
    let httpParams = new HttpParams().append("cron", cron);
    return this.http.patch<void>(this.baseUrl + '/' + id, {}, { params: httpParams });
  }
}
