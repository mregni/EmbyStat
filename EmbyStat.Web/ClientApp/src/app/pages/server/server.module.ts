import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';

import { SharedModule } from '../../shared/shared.module';
import { ServerOverviewComponent } from './server-overview/server-overview.component';

@NgModule({
  declarations: [ServerOverviewComponent],
  imports: [
    CommonModule,
    TranslateModule,
    SharedModule
  ]
})
export class ServerModule { }
