import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { SharedModule } from '../shared/shared.module';

import { LogsOverviewComponent } from './logs-overview/logs-overview.component';
import { LogService } from './service/log.service';

@
NgModule({
  imports: [
    CommonModule,
    SharedModule,
    TranslateModule
  ],
  providers: [
    LogService
  ],
  declarations: [LogsOverviewComponent]
})
export class LogsModule { }
