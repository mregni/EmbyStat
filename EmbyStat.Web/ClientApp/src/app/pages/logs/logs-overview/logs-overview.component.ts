import { Observable } from 'rxjs';

import { Component, OnInit } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';

import { LogFile } from '../../../shared/models/logs/log-file';
import { LogService } from '../service/logs.service';

@Component({
  selector: 'es-logs-overview',
  templateUrl: './logs-overview.component.html',
  styleUrls: ['./logs-overview.component.scss']
})
export class LogsOverviewComponent implements OnInit {
  logs$: Observable<LogFile[]>;

  constructor(private readonly logService: LogService) {
    this.logs$ = this.logService.getLogFiles();
  }

  downloadLog(fileName: string, anonymous: boolean): string {
      return `/api/log/download/${fileName}.log?anonymous=${String(anonymous)}`;
  }

  convertToSize(value: number): string {
    if (value < 1024) {
      return `${String(value)} b`;
    } else if (value < 1024 * 1024) {
      return `${Math.floor(value / 1024)} Kb`;
    } else {
      return `${Math.floor(value / (1024 * 1024))} Mb`;
    }
  }

  ngOnInit() {
  }
}
