import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ConfigurationComponent } from './configuration.component';
import { ConfigurationService } from './service/configuration.service';
import { ConfigurationFacade } from '../states/configuration/configuration.facade';

@NgModule({
  imports: [
    CommonModule
  ],
  providers: [
    ConfigurationService,
    ConfigurationFacade
  ],
  declarations: [ConfigurationComponent]
})
export class ConfigurationModule { }
