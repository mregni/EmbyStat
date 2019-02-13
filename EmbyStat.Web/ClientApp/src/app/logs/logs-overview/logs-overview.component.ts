import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';

import { LogService } from '../service/log.service';
import { LogFile } from '../models/logFile';

@Component({
  selector: 'app-logs-overview',
  templateUrl: './logs-overview.component.html',
  styleUrls: ['./logs-overview.component.scss']
})
export class LogsOverviewComponent implements OnInit {
  logs$: Observable<LogFile[]>;

  constructor(private logService: LogService) {
    this.logs$ = this.logService.getLogFiles();
  }

  downloadLog(fileName: string, anonymous: boolean): string {
    return '/api/log/download/' + fileName + '?anonymous=' + anonymous;
  }

  convertToSize(value: number): string {
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
