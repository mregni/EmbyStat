import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { SharedModule } from '../shared/shared.module';
import { MomentModule } from 'angular2-moment';

import { TaskComponent } from './task.component';
import { TaskService } from './service/task.service';
import { TaskFacade } from './state/facade.task';

@NgModule({
  imports: [
    CommonModule,
    TranslateModule,
    SharedModule,
    MomentModule
  ],
  providers: [
    TaskService,
    TaskFacade
  ],
  declarations: [TaskComponent]
})
export class TaskModule { }
