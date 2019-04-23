import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';

import { SharedModule } from '../../shared/shared.module';
import { SettingsContainerComponent } from './settings-container/settings-container.component';
import { SettingsGeneralComponent } from './settings-general/settings-general.component';

@NgModule({
  declarations: [
    SettingsContainerComponent,
    SettingsGeneralComponent
  ],
  imports: [
    CommonModule,
    SharedModule
  ]
})
export class SettingsModule { }
