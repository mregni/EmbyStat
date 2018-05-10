import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';
import { SharedModule } from '../shared/shared.module';

import { ShowService } from './service/show.service';

import { ShowOverviewComponent } from './show-overview/show-overview.component';
import { ShowGeneralComponent } from './show-general/show-general.component';
import { ShowChartsComponent } from './show-charts/show-charts.component';
import { ShowPeopleComponent } from './show-people/show-people.component';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    TranslateModule,
    SharedModule
  ],
  providers: [
    ShowService
  ],
  declarations: [ShowOverviewComponent, ShowGeneralComponent, ShowChartsComponent, ShowPeopleComponent]
})
export class ShowModule { }
