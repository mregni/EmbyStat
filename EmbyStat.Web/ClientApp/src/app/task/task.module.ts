import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { SharedModule } from '../shared/shared.module';

import { TaskService } from './service/task.service';
import { TriggerDialogComponent } from './trigger-dialog/trigger-dialog.component';

import { TicksToTime } from '../shared/pipes/ticks-to-time';
import { DateToHoursAgo } from '../shared/pipes/time-to-hours-ago';
import { DateToMinutesAgo } from '../shared/pipes/time-to-minutes-ago';
import { DateToSecondsAgo } from '../shared/pipes/time-to-seconds-ago';
import { TaskOverviewComponent } from './task-overview/task-overview.component';

@NgModule({
  imports: [
    CommonModule,
    TranslateModule,
    SharedModule
  ],
  providers: [
    TaskService
  ],
  declarations: [
    TriggerDialogComponent,
    TicksToTime,
    DateToHoursAgo,
    DateToMinutesAgo,
    DateToSecondsAgo,
    TaskOverviewComponent
  ],
  entryComponents: [TriggerDialogComponent]
})
export class TaskModule { }
