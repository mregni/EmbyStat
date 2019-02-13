import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { SharedModule } from '../shared/shared.module';
import { EmbyService } from '../shared/services/emby.service';
import { PluginOverviewComponent } from './plugin-overview/plugin-overview.component';

@NgModule({
  imports: [
    CommonModule,
    TranslateModule,
    SharedModule
  ],
  providers: [
    EmbyService
  ],
  declarations: [
    PluginOverviewComponent]
})
export class PluginModule { }
