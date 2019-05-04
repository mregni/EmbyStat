import { ChartModule } from 'angular2-chartjs';
import { CountUpModule } from 'countup.js-angular2';
import { ClickOutsideModule } from 'ng-click-outside';
import { MomentModule } from 'ngx-moment';
import {
    PERFECT_SCROLLBAR_CONFIG, PerfectScrollbarConfigInterface, PerfectScrollbarModule
} from 'ngx-perfect-scrollbar';
import { NgScrollbarModule } from 'ngx-scrollbar';

import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { ModuleWithProviders, NgModule, NO_ERRORS_SCHEMA } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { NgSelectModule } from '@ng-select/ng-select';
import { TranslateModule } from '@ngx-translate/core';

import { CollectionService } from './behaviors/collection.service';
import { NumberCardComponent } from './components/cards/number-card/number-card.component';
import { TimeCardComponent } from './components/cards/time-card/time-card.component';
import {
    SimpleBarChartComponent
} from './components/charts/simple-bar-chart/simple-bar-chart.component';
import {
    CollectionDropdownComponent
} from './components/collection-dropdown/collection-dropdown.component';
import { LoaderComponent } from './components/loader/loader.component';
import { MoviePosterComponent } from './components/posters/movie-poster/movie-poster.component';
import { PersonListComponent } from './components/posters/person-list/person-list.component';
import { PersonPosterComponent } from './components/posters/person-poster/person-poster.component';
import { CapitalizeFirstPipe } from './pipes/capitalize-first.pipe';
import { ToShorterStringPipe } from './pipes/to-shorter-string.pipe';
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
    MomentModule,
    NgSelectModule,
    FormsModule,
    TranslateModule,
    CountUpModule,
    ChartModule,
    NgScrollbarModule
  ],
  declarations: [
    CapitalizeFirstPipe,
    ToShorterStringPipe,
    CollectionDropdownComponent,
    NumberCardComponent,
    TimeCardComponent,
    LoaderComponent,
    MoviePosterComponent,
    SimpleBarChartComponent,
    PersonPosterComponent,
    PersonListComponent
  ],
  exports: [
    NgbModule,
    RouterModule,
    HttpClientModule,
    PerfectScrollbarModule,
    ClickOutsideModule,
    MomentModule,
    NgScrollbarModule,
    NgSelectModule,
    FormsModule,
    CountUpModule,
    CollectionDropdownComponent,
    NumberCardComponent,
    TimeCardComponent,
    LoaderComponent,
    MoviePosterComponent,
    SimpleBarChartComponent,
    PersonPosterComponent,
    PersonListComponent,
    CapitalizeFirstPipe,
    ToShorterStringPipe
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
        EmbyService,
        CollectionService
      ]
    };
  }
}
