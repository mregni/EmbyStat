import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../shared/shared.module';

import { ServerFacade } from './state/facade.server';
import { ServerOverviewComponent } from './server-overview/server-overview.component';

@NgModule({
  imports: [
    CommonModule,
    SharedModule
  ],
  providers: [
    ServerFacade
  ],
  declarations: [
    ServerOverviewComponent
  ]
})
export class ServerModule { }
