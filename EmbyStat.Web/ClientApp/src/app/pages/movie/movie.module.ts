import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';

import { SharedModule } from '../../shared/shared.module';
import { MovieContainerComponent } from './movie-container/movie-container.component';
import { MovieOverviewComponent } from './movie-overview/movie-overview.component';

@NgModule({
  declarations: [
    MovieContainerComponent,
    MovieOverviewComponent
  ],
  imports: [
    CommonModule,
    SharedModule
  ]
})
export class MovieModule { }
