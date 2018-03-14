import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MaterialModule } from './material.module';
import { TranslateModule } from '@ngx-translate/core';

import { ToolbarComponent } from './toolbar/toolbar.component';
import { SideNavComponent } from './side-nav/side-nav.component';
import { ShutdownComponent } from './dialog/shutdown/shutdown.dialog';

import { ToastService } from './services/toast.service';

@NgModule({
  imports: [
    CommonModule,
    MaterialModule,
    RouterModule,
    TranslateModule.forChild()
  ],
  exports: [
    ToolbarComponent,
    SideNavComponent,
    MaterialModule
  ],
  declarations: [
    ToolbarComponent,
    SideNavComponent,
    ShutdownComponent
  ],
  providers: [
    ToastService
  ], entryComponents: [ShutdownComponent]
})
export class SharedModule { }
