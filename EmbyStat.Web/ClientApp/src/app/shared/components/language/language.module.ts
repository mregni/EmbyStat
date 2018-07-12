import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { LanguageService } from './service/language.service';
import { LanguageFacade } from './state/facade.language';
import { LanguageComponent } from './language-overview/language.component';

@NgModule({
  imports: [
    CommonModule
  ],
  exports: [
    LanguageComponent
  ],
  declarations: [
    LanguageComponent
  ],
  providers: [
    LanguageService,
    LanguageFacade
  ]
})
export class LanguageModule { }
