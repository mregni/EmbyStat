import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { SharedModule } from '../shared/shared.module';

import { JobService } from './service/job.service';
import { TriggerDialogComponent } from './trigger-dialog/trigger-dialog.component';

import { TicksToTime } from '../shared/pipes/ticks-to-time';
import { DateToHoursAgo } from '../shared/pipes/time-to-hours-ago';
import { DateToMinutesAgo } from '../shared/pipes/time-to-minutes-ago';
import { DateToSecondsAgo } from '../shared/pipes/time-to-seconds-ago';
import { JobsOverviewComponent } from './jobs-overview/jobs-overview.component';
import { JobItemComponent } from './job-item/job-item.component';

@NgModule({
  imports: [
    CommonModule,
    TranslateModule,
    SharedModule
  ],
  providers: [
    JobService
  ],
  declarations: [
    TriggerDialogComponent,
    TicksToTime,
    DateToHoursAgo,
    DateToMinutesAgo,
    DateToSecondsAgo,
    JobsOverviewComponent,
    JobItemComponent
  ],
  entryComponents: [TriggerDialogComponent]
})
export class JobsModule { }
