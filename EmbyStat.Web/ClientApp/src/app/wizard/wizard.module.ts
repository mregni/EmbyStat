import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../shared/shared.module';
import { TranslateModule } from '@ngx-translate/core';
import { WizardComponent } from './wizard.component';
import { SystemFacade } from '../system/state/facade.system';
import { SystemService } from '../system/service/system.service';

@NgModule({
  imports: [
    CommonModule,
    TranslateModule,
    SharedModule,
    FormsModule,
    ReactiveFormsModule
  ],
  providers: [
    SystemService,
    SystemFacade
  ],
  declarations: [WizardComponent]
})
export class WizardModule { }
