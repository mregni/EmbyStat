import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MaterialModule } from './material.module';
import { TranslateModule } from '@ngx-translate/core';
import { CountUpModule } from 'countup.js-angular2';

import { ToolbarComponent } from './toolbar/toolbar.component';
import { SideNavComponent } from './side-nav/side-nav.component';
import { CardComponent } from './components/card/card.component';
import { CardTimespanComponent } from './components/card-timespan/card-timespan.component';
import { CardNumberComponent } from './components/card-number/card-number.component';
import { PosterComponent } from './components/poster/poster.component';

import { ToastService } from './services/toast.service';

@NgModule({
  imports: [
    CommonModule,
    MaterialModule,
    RouterModule,
    CountUpModule,
    TranslateModule.forChild()
  ],
  exports: [
    SideNavComponent,
    ToolbarComponent,
    MaterialModule,
    CardComponent,
    CardTimespanComponent,
    CardNumberComponent,
    PosterComponent
  ],
  declarations: [
    ToolbarComponent,
    SideNavComponent,
    CardComponent,
    CardTimespanComponent,
    CardNumberComponent,
    PosterComponent
  ],
  providers: [
    ToastService
  ],
  entryComponents: []
})
export class SharedModule { }
