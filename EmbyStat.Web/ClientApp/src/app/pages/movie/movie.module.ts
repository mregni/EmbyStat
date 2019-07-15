import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';

import { SharedModule } from '../../shared/shared.module';
import { MovieOverviewComponent } from './movie-overview/movie-overview.component';
import { MovieService } from './service/movie.service';


@NgModule({
  declarations: [
    MovieOverviewComponent
  ],
  imports: [
    CommonModule,
    SharedModule,
    TranslateModule
  ],
  providers: [
    MovieService
  ]
})
export class MovieModule { }
