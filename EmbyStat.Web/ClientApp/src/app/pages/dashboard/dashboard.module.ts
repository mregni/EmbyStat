import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';

import { SharedModule } from '../../shared/shared.module';
import { DashboardOverviewComponent } from './dashboard-overview/dashboard-overview.component';

@NgModule({
  declarations: [DashboardOverviewComponent],
  imports: [
    CommonModule,
    TranslateModule,
    SharedModule
  ]
})
export class DashboardModule { }
