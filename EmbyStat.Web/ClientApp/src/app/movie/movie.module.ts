import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';
import { SharedModule } from '../shared/shared.module';
import { MovieService } from './service/movie.service';
import { MovieFacade } from './state/facade.movie';

import { MovieStatComponent } from './movie-stat/movie-stat.component';

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
  declarations: [MovieStatComponent]
})
export class MovieModule { }
