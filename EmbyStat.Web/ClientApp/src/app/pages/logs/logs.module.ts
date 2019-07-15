import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';

import { SharedModule } from '../../shared/shared.module';
import { LogsOverviewComponent } from './logs-overview/logs-overview.component';
import { LogService } from './service/logs.service';

@NgModule({
  declarations: [LogsOverviewComponent],
  imports: [
    CommonModule,
    TranslateModule,
    SharedModule
  ],
  providers: [
    LogService
  ]
})
export class LogsModule { }
