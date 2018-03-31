import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MaterialModule } from './material.module';
import { TranslateModule } from '@ngx-translate/core';

import { ToolbarComponent } from './toolbar/toolbar.component';
import { SideNavComponent } from './side-nav/side-nav.component';

import { ToastService } from './services/toast.service';

@NgModule({
  imports: [
    CommonModule,
    MaterialModule,
    RouterModule,
    TranslateModule.forChild()
  ],
  exports: [
    SideNavComponent,
    ToolbarComponent,
    MaterialModule
  ],
  declarations: [
    ToolbarComponent,
    SideNavComponent
  ],
  providers: [
    ToastService
  ], entryComponents: []
})
export class SharedModule { }
