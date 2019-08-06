import { ChartModule } from 'angular2-chartjs';
import { CountUpModule } from 'countup.js-angular2';
import { MomentModule } from 'ngx-moment';
import { NgScrollbarModule } from 'ngx-scrollbar';

import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { ModuleWithProviders, NgModule, NO_ERRORS_SCHEMA } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { TranslateModule } from '@ngx-translate/core';

import { NumberCardComponent } from './components/cards/number-card/number-card.component';
import { TimeCardComponent } from './components/cards/time-card/time-card.component';
import { UserCardComponent } from './components/cards/user-card/user-card.component';
import { SimpleChartComponent } from './components/charts/simple-chart/simple-chart.component';
import {
    CollectionSelectorComponent
} from './components/collection-selector/collection-selector.component';
import { LanguageComponent } from './components/language/language.component';
import { LoaderComponent } from './components/loader/loader.component';
import { MoviePosterComponent } from './components/posters/movie-poster/movie-poster.component';
import { PersonListComponent } from './components/posters/person-list/person-list.component';
import { PersonPosterComponent } from './components/posters/person-poster/person-poster.component';
import { ShowPosterComponent } from './components/posters/show-poster/show-poster.component';
import { SideNavigationComponent } from './components/side-navigation/side-navigation.component';
import { ToolbarComponent } from './components/toolbar/toolbar.component';
import { UpdateOverlayComponent } from './components/update-overlay/update-overlay.component';
import { NoTypeFoundDialog } from './dialogs/no-type-found/no-type-found.component';
import { NoUsersFoundDialog } from './dialogs/no-users-found-dialog/no-users-found-dialog';
import { SyncIsRunningDialog } from './dialogs/sync-is-running/sync-is-running.component';
import { DisableControlDirective } from './directives/disable-control.directive';
import { EmbyServerInfoFacade } from './facades/emby-server.facade';
import { SettingsFacade } from './facades/settings.facade';
import { SyncGuard } from './guards/sync.guard';
import { MaterialModule } from './material.module';
import { CapitalizeFirstPipe } from './pipes/capitalize-first.pipe';
import { TicksToTimePipe } from './pipes/ticks-to-time.pipe';
import { DateToHoursAgoPipe } from './pipes/time-to-hours-ago.pipe';
import { DateToMinutesAgoPipe } from './pipes/time-to-minutes-ago.pipe';
import { DateToSecondsAgoPipe } from './pipes/time-to-seconds-ago.pipe';
import { ToShorterStringPipe } from './pipes/to-shorter-string.pipe';
import { EmbyService } from './services/emby.service';
import { JobSocketService } from './services/job-socket.service';
import { PageService } from './services/page.service';
import { SettingsService } from './services/settings.service';
import { SideBarService } from './services/side-bar.service';
import { SystemService } from './services/system.service';
import { ToastService } from './services/toast.service';
import { UpdateOverlayService } from './services/update-overlay.service';
import { UpdateService } from './services/update.service';
import { UserService } from './services/user.service';

@NgModule({
  imports: [
    CommonModule,
    RouterModule,
    HttpClientModule,
    FormsModule,
    TranslateModule,
    CountUpModule,
    FormsModule,
    ReactiveFormsModule,
    MaterialModule,
    MomentModule,
    NgScrollbarModule,
    FontAwesomeModule,
    ChartModule
  ],
  declarations: [
    CapitalizeFirstPipe,
    ToShorterStringPipe,
    DateToSecondsAgoPipe,
    DateToMinutesAgoPipe,
    DateToHoursAgoPipe,
    TicksToTimePipe,
    NumberCardComponent,
    TimeCardComponent,
    LoaderComponent,
    SimpleChartComponent,
    PersonPosterComponent,
    PersonListComponent,
    ToolbarComponent,
    SideNavigationComponent,
    CollectionSelectorComponent,
    LanguageComponent,
    DisableControlDirective,
    MoviePosterComponent,
    ShowPosterComponent,
    UserCardComponent,
    UpdateOverlayComponent,
    NoTypeFoundDialog,
    SyncIsRunningDialog,
    NoUsersFoundDialog
  ],
  exports: [
    RouterModule,
    FormsModule,
    ReactiveFormsModule,
    CountUpModule,
    HttpClientModule,
    MaterialModule,
    MomentModule,
    NgScrollbarModule,
    FontAwesomeModule,
    NumberCardComponent,
    TimeCardComponent,
    LoaderComponent,
    LanguageComponent,
    SimpleChartComponent,
    PersonPosterComponent,
    MoviePosterComponent,
    ShowPosterComponent,
    PersonListComponent,
    ToolbarComponent,
    SideNavigationComponent,
    CollectionSelectorComponent,
    UserCardComponent,
    CapitalizeFirstPipe,
    ToShorterStringPipe,
    DateToSecondsAgoPipe,
    DateToMinutesAgoPipe,
    DateToHoursAgoPipe,
    TicksToTimePipe,
    DisableControlDirective
  ],
  entryComponents: [
    NoTypeFoundDialog,
    SyncIsRunningDialog,
    NoUsersFoundDialog,
    UpdateOverlayComponent
  ],
  schemas: [NO_ERRORS_SCHEMA]
})
export class SharedModule {
  static forRoot(): ModuleWithProviders {
    return {
      ngModule: SharedModule,
      providers: [
        SettingsService,
        SettingsFacade,
        EmbyService,
        EmbyServerInfoFacade,
        ToastService,
        JobSocketService,
        SideBarService,
        SystemService,
        UpdateOverlayService,
        UpdateService,
        UserService,
        PageService,
        SyncGuard
      ]
    };
  }
}
