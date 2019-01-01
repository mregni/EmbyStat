import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs/Observable';

@Injectable()
export class UpdateService {
  private readonly startUpdateUrl: string = '/update/checkforupdate';

  constructor(private http: HttpClient) { }

  startEmbyStatUpdate(): Observable<void> {
    return this.http.get<void>('/api' + this.startUpdateUrl);
  }
}
