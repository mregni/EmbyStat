import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { SharedModule } from '../shared/shared.module';
import { PluginService } from './service/plugin.service';
import { PluginOverviewComponent } from './plugin-overview/plugin-overview.component';

@NgModule({
  imports: [
    CommonModule,
    TranslateModule,
    SharedModule
  ],
  providers: [
    PluginService
  ],
  declarations: [
    PluginOverviewComponent]
})
export class PluginModule { }
