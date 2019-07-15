import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';

import { SharedModule } from '../../shared/shared.module';
import { AboutOverviewComponent } from './about-overview/about-overview.component';
import { AboutService } from './services/about.service';

@NgModule({
  declarations: [
    AboutOverviewComponent
  ],
  imports: [
    CommonModule,
    SharedModule,
    TranslateModule
  ],
  providers: [
    AboutService
  ]
})
export class AboutModule { }
