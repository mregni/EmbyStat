import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../shared/shared.module';
import { TranslateModule } from '@ngx-translate/core';
import { WizardOverviewComponent } from './wizard-overview/wizard-overview.component';
import { WizardStateService } from './services/wizard-state.service';

@NgModule({
  imports: [
    CommonModule,
    TranslateModule,
    SharedModule,
    FormsModule,
    ReactiveFormsModule
  ],
  providers: [WizardStateService],
  declarations: [WizardOverviewComponent]
})
export class WizardModule { }
