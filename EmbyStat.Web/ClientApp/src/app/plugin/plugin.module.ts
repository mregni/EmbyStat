import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { SharedModule } from '../shared/shared.module';
import { PluginComponent } from './plugin.component';
import { PluginService } from './service/plugin.service';
import { PluginFacade } from './state/facade.plugin';

@NgModule({
  imports: [
    CommonModule,
    TranslateModule,
    SharedModule
  ],
  providers: [
    PluginService,
    PluginFacade
  ],
  declarations: [
    PluginComponent]
})
export class PluginModule { }
