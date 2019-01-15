import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../shared/shared.module';
import { TranslateModule } from '@ngx-translate/core';
import { DashboardOverviewComponent } from './dashboard-overview/dashboard-overview.component';

@NgModule({
  imports: [
    CommonModule,
    TranslateModule,
    SharedModule
  ],
  declarations: [DashboardOverviewComponent]
})
export class DashboardModule { }
