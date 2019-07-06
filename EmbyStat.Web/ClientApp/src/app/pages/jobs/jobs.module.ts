import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';

import { JobService } from '../../shared/services/job.service';
import { SharedModule } from '../../shared/shared.module';
import { JobItemComponent } from './components/job-item/job-item.component';
import { TriggerDialogComponent } from './components/trigger-dialog/trigger-dialog.component';
import { JobsOverviewComponent } from './jobs-overview/jobs-overview.component';

@NgModule({
  declarations: [
    JobsOverviewComponent,
    JobItemComponent,
    TriggerDialogComponent],
  imports: [
    CommonModule,
    SharedModule,
    TranslateModule
  ],
  providers: [
    JobService
  ],
  entryComponents: [
    TriggerDialogComponent
  ]
})
export class JobsModule { }
