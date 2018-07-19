import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';
import { SharedModule } from '../shared/shared.module';
import { MovieService } from './service/movie.service';
import { MovieChartsService } from './service/movie-charts.service';
import { MovieFacade } from './state/facade.movie';

import { MovieOverviewComponent } from './movie-overview/movie-overview.component';
import { MovieGeneralComponent } from './movie-general/movie-general.component';
import { MoviePeopleComponent } from './movie-people/movie-people.component';
import { MovieChartsComponent } from './movie-charts/movie-charts.component';
import { MovieSuspiciousComponent } from './movie-suspicious/movie-suspicious.component';


@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    TranslateModule,
    SharedModule
  ],
  providers: [
    MovieService,
    MovieChartsService,
    MovieFacade
  ],
  declarations: [MovieOverviewComponent, MovieGeneralComponent, MoviePeopleComponent, MovieChartsComponent, MovieSuspiciousComponent]
})
export class MovieModule { }
