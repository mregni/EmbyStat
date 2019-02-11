import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../shared/shared.module';

import { SettingsService } from './settings.service';
import { SettingsFacade } from './state/facade.settings';
import { SettingsEmbyComponent } from './settings-emby/settings-emby.component';
import { SettingsGeneralComponent } from './settings-general/settings-general.component';
import { SettingsMoviesComponent } from './settings-movies/settings-movies.component';
import { SettingsOverviewComponent } from './settings-overview/settings-overview.component';
import { SettingsShowsComponent } from './settings-shows/settings-shows.component';
import { SettingsUpdatesComponent } from './settings-updates/settings-updates.component';

@NgModule({
  imports: [
    CommonModule,
    TranslateModule,
    SharedModule,
    ReactiveFormsModule
  ],
  providers: [
    SettingsService,
    SettingsFacade
  ],
  declarations: [
    SettingsEmbyComponent,
    SettingsGeneralComponent,
    SettingsMoviesComponent,
    SettingsOverviewComponent,
    SettingsShowsComponent,
    SettingsUpdatesComponent
  ]
})
export class SettingsModule { }
