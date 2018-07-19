import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs/Observable';

import { LogFacade } from './state/facade.log';
import { LogFile } from './models/logFile';

@Component({
  selector: 'app-logs',
  templateUrl: './logs.component.html',
  styleUrls: ['./logs.component.scss']
})
export class LogsComponent implements OnInit {
  public logs$: Observable<LogFile[]>;

  constructor(private logFacade: LogFacade) {
    this.logs$ = this.logFacade.getLogFiles();
  }

  public downloadLog(fileName: string): string {
    return '/api/log/download/' + fileName + '?anonymous=false';
  }

  public downloadAnonymousLog(fileName: string): string {
    return '/api/log/download/' + fileName + '?anonymous=true';
  }

  public convertToSize(value: number): string {
    if (value < 1024) {
      return value + ' b';
    } else if (value < 1024 * 1024) {
      return Math.floor(value / 1024) + ' Kb';
    } else {
      return Math.floor(value / (1024 * 1024)) + 'Mb';
    }
  }

  ngOnInit() {
  }

}
