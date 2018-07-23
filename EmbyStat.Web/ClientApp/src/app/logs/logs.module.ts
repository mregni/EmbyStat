import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { SharedModule } from '../shared/shared.module';

import { LogsComponent } from './logs.component';
import { LogService } from './service/log.service';
import { LogFacade } from './state/facade.log';

@
NgModule({
  imports: [
    CommonModule,
    SharedModule,
    TranslateModule
  ],
  providers: [
    LogService,
    LogFacade
  ],
  declarations: [LogsComponent]
})
export class LogsModule { }
