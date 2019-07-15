import { SharedModule } from 'src/app/shared/shared.module';

import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';

import { WizardOverviewComponent } from './wizard-overview/wizard-overview.component';

@NgModule({
  declarations: [WizardOverviewComponent],
  imports: [
    CommonModule,
    SharedModule,
    TranslateModule
  ]
})
export class WizardModule { }
