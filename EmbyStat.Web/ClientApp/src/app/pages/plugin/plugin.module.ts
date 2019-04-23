import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';

import { SharedModule } from '../../shared/shared.module';
import { PluginOverviewComponent } from './plugin-overview/plugin-overview.component';

@NgModule({
  declarations: [PluginOverviewComponent],
  imports: [
    CommonModule,
    TranslateModule,
    SharedModule
  ]
})
export class PluginModule { }
