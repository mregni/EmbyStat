import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MaterialModule } from './material.module';
import { TranslateModule } from '@ngx-translate/core';
import { CountUpModule } from 'countup.js-angular2';
import { MomentModule } from 'ngx-moment';
import { NgxChartsModule } from '@swimlane/ngx-charts';

import { ToolbarComponent } from './toolbar/toolbar.component';
import { SideNavComponent } from './side-nav/side-nav.component';
import { CardComponent } from './components/card/card.component';
import { CardTimespanComponent } from './components/card-timespan/card-timespan.component';
import { CardNumberComponent } from './components/card-number/card-number.component';
import { PosterComponent } from './components/poster/poster.component';
import { PersonPosterComponent } from './components/person-poster/person-poster.component';

import { CapitalizeFirstPipe } from './pipes/capitalizefirst.pipe';

import { ToastService } from './services/toast.service';

@NgModule({
  imports: [
    CommonModule,
    MaterialModule,
    RouterModule,
    CountUpModule,
    MomentModule,
    NgxChartsModule,
    TranslateModule.forChild()
  ],
  exports: [
    SideNavComponent,
    ToolbarComponent,
    MaterialModule,
    MomentModule,
    NgxChartsModule,
    CardComponent,
    CardTimespanComponent,
    CardNumberComponent,
    PosterComponent,
    PersonPosterComponent,
    CapitalizeFirstPipe
  ],
  declarations: [
    ToolbarComponent,
    SideNavComponent,
    CardComponent,
    CardTimespanComponent,
    CardNumberComponent,
    PosterComponent,
    PersonPosterComponent,
    CapitalizeFirstPipe
  ],
  providers: [
    ToastService
  ],
  entryComponents: []
})
export class SharedModule { }
