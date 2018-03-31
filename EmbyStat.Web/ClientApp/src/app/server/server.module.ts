import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ServerComponent } from './server.component';

import { ServerService } from './service/server.service';
import { ServerFacade } from './state/facade.server';

@NgModule({
  imports: [
    CommonModule
  ],
  providers: [
    ServerService,
    ServerFacade
  ],
  declarations: [ServerComponent]
})
export class ServerModule { }
