import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';

import { SharedModule } from '../../shared/shared.module';
import { ShowService } from './service/show.service';
import { ShowOverviewComponent } from './show-overview/show-overview.component';

@NgModule({
  declarations: [
    ShowOverviewComponent
  ],
  imports: [
    CommonModule,
    TranslateModule,
    SharedModule
  ],
  providers: [
    ShowService
  ]
})
export class ShowModule { }
