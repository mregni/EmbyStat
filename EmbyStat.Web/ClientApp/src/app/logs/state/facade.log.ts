import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';

import { LogService } from '../service/log.service';

import { LogFile } from '../models/logFile';

@Injectable()
export class LogFacade {
  constructor(private logService: LogService) { }

  public logs$: Observable<LogFile[]>;

  getLogFiles(): Observable<LogFile[]> {
    this.logs$ = this.logService.getLogFiles();
    return this.logs$;
  }

  downloadLog(fileName: string): void {
    this.logService.downloadLog(fileName).subscribe();
  }
}

