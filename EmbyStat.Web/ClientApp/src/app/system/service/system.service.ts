import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { Observable } from 'rxjs/Observable';

@Injectable()
export class SystemService {
  private readonly shutdownServerUrl: string = '/system/shutdown';

  constructor(private http: HttpClient) {

  }
}
