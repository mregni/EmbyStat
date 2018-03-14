import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { SharedModule } from '../shared/shared.module';
import { ConfigurationComponent } from './configuration.component';
import { ConfigurationService } from './service/configuration.service';
import { ConfigurationFacade } from './state/facade.configuration';

@NgModule({
  imports: [
    CommonModule,
    TranslateModule,
    SharedModule
  ],
  providers: [
    ConfigurationService,
    ConfigurationFacade
  ],
  declarations: [ConfigurationComponent]
})
export class ConfigurationModule { }
