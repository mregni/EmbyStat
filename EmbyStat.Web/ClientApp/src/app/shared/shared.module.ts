import { ClickOutsideModule } from 'ng-click-outside';
import { MomentModule } from 'ngx-moment';
import {
    PERFECT_SCROLLBAR_CONFIG, PerfectScrollbarConfigInterface, PerfectScrollbarModule
} from 'ngx-perfect-scrollbar';

import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { ModuleWithProviders, NgModule, NO_ERRORS_SCHEMA } from '@angular/core';
import { RouterModule } from '@angular/router';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

import { CapitalizeFirstPipe } from './pipes/capitalize-first.pipe';
import { EmbyService } from './services/emby.service';
import { SettingsService } from './services/settings.service';
import { TitleService } from './services/title.service';

const DEFAULT_PERFECT_SCROLLBAR_CONFIG: PerfectScrollbarConfigInterface = {
  suppressScrollX: true
};

@NgModule({
  imports: [
    CommonModule,
    NgbModule,
    HttpClientModule,
    PerfectScrollbarModule,
    ClickOutsideModule,
    RouterModule,
    MomentModule
  ],
  declarations: [
    CapitalizeFirstPipe
  ],
  exports: [
    NgbModule,
    RouterModule,
    HttpClientModule,
    PerfectScrollbarModule,
    ClickOutsideModule,
    MomentModule,
    CapitalizeFirstPipe
  ],
  providers: [
    {
      provide: PERFECT_SCROLLBAR_CONFIG,
      useValue: DEFAULT_PERFECT_SCROLLBAR_CONFIG
    }
  ],
  schemas: [NO_ERRORS_SCHEMA]
})
export class SharedModule {
  static forRoot(): ModuleWithProviders {
    return {
      ngModule: SharedModule,
      providers: [
        TitleService,
        SettingsService,
        EmbyService
       ]
    };
  }
}
