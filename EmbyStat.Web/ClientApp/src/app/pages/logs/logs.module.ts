import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';

import { SharedModule } from '../../shared/shared.module';
import { LogsOverviewComponent } from './logs-overview/logs-overview.component';
import { LogService } from './service/logs.service';

@NgModule({
  declarations: [LogsOverviewComponent],
  imports: [
    CommonModule,
    SharedModule
  ],
  providers: [
    LogService
  ]
})
export class LogsModule { }
