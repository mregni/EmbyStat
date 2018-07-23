import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../shared/shared.module';
import { ConfigurationService } from './service/configuration.service';
import { ConfigurationFacade } from './state/facade.configuration';
import { ConfigurationEmbyComponent } from './configuration-emby/configuration-emby.component';
import { ConfigurationGeneralComponent } from './configuration-general/configuration-general.component';
import { ConfigurationMoviesComponent } from './configuration-movies/configuration-movies.component';
import { ConfigurationOverviewComponent } from './configuration-overview/configuration-overview.component';
import { ConfigurationShowsComponent } from './configuration-shows/configuration-shows.component';

@NgModule({
  imports: [
    CommonModule,
    TranslateModule,
    SharedModule,
    ReactiveFormsModule
  ],
  providers: [
    ConfigurationService,
    ConfigurationFacade
  ],
  declarations: [ConfigurationEmbyComponent, ConfigurationGeneralComponent
    , ConfigurationMoviesComponent, ConfigurationOverviewComponent, ConfigurationShowsComponent]
})
export class ConfigurationModule { }
