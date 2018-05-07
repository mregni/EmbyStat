import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { SharedModule } from '../shared/shared.module';

import { TaskComponent } from './task.component';
import { TaskService } from './service/task.service';
import { TaskFacade } from './state/facade.task';
import { TriggerDialogComponent } from './trigger-dialog/trigger-dialog.component';

import { TicksToTime } from '../shared/pipes/ticks-to-time';
import { DateToHoursAgo } from '../shared/pipes/time-to-hours-ago';
import { DateToMinutesAgo } from '../shared/pipes/time-to-minutes-ago';
import { DateToSecondsAgo } from '../shared/pipes/time-to-seconds-ago';

@NgModule({
  imports: [
    CommonModule,
    TranslateModule,
    SharedModule
  ],
  providers: [
    TaskService,
    TaskFacade
  ],
  declarations: [
    TaskComponent,
    TriggerDialogComponent,
    TicksToTime,
    DateToHoursAgo,
    DateToMinutesAgo,
    DateToSecondsAgo
  ],
  entryComponents: [TriggerDialogComponent]
})
export class TaskModule { }
