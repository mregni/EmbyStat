import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';

import { SharedModule } from '../../shared/shared.module';
import { SettingsGeneralComponent } from './components/settings-general/settings-general.component';
import { SettingsOverviewComponent } from './settings-overview/settings-overview.component';
import { SettingsEmbyComponent } from './components/settings-emby/settings-emby.component';
import { SettingsMovieComponent } from './components/settings-movie/settings-movie.component';
import { SettingsShowComponent } from './components/settings-show/settings-show.component';
import { SettingsUpdateComponent } from './components/settings-update/settings-update.component';

@NgModule({
  declarations: [
    SettingsOverviewComponent,
    SettingsGeneralComponent,
    SettingsEmbyComponent,
    SettingsMovieComponent,
    SettingsShowComponent,
    SettingsUpdateComponent
  ],
  imports: [
    CommonModule,
    SharedModule,
    TranslateModule
  ]
})
export class SettingsModule { }
