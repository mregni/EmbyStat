import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';
import { SharedModule } from '../shared/shared.module';
import { MovieService } from './service/movie.service';
import { MovieFacade } from './state/facade.movie';

import { MovieStatComponent } from './movie-stat/movie-stat.component';
import { MovieGeneralComponent } from './movie-general/movie-general.component';
import { MoviePeopleComponent } from './movie-people/movie-people.component';
import { MovieChartsComponent } from './movie-charts/movie-charts.component';

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
    MovieFacade
  ],
  declarations: [MovieStatComponent, MovieGeneralComponent, MoviePeopleComponent, MovieChartsComponent]
})
export class MovieModule { }
