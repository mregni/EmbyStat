import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MaterialModule } from './material.module';
import { TranslateModule } from '@ngx-translate/core';
import { CountUpModule } from 'countup.js-angular2';
import { MomentModule } from 'ngx-moment';
import { NgxChartsModule } from '@swimlane/ngx-charts';
import { LanguageModule } from './components/language/language.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { CardComponent } from './components/card/card.component';
import { CardTimespanComponent } from './components/card-timespan/card-timespan.component';
import { CardNumberComponent } from './components/card-number/card-number.component';
import { MoviePosterComponent } from './components/movie-poster/movie-poster.component';
import { PersonPosterComponent } from './components/person-poster/person-poster.component';
import { ShowPosterComponent } from './components/show-poster/show-poster.component';
import { LoaderComponent } from './components/loader/loader.component';
import { CollectionSelectorComponent } from './components/collection-selector/collection-selector.component';
import { ToolbarComponent } from './components/toolbar/toolbar.component';
import { UpdateOverlayComponent } from './components/update-overlay/update-overlay.component';
import { SideNavigationComponent } from './components/side-navigation/side-navigation.component';
import { CardUserComponent } from '../shared/components/card-user/card-user.component';

import { CapitalizeFirstPipe } from './pipes/capitalize-first.pipe';
import { ToShorterStringPipe } from './pipes/shorten-string.pipe';

import { ToastService } from './services/toast.service';
import { JobSocketService } from './services/job-socket.service';
import { EmbyService } from './services/emby.service';
import { SystemService } from './services/system.service';
import { UpdateOverlayService } from './services/update-overlay.service';
import { SideBarService } from './services/side-bar.service';
import { UpdateService } from './services/update.service';

import { NoTypeFoundDialog } from './dialogs/no-type-found/no-type-found.component';
import { SyncIsRunningDialog } from './dialogs/sync-is-running/sync-is-running.component';
import { NoUsersFoundDialogComponent } from './dialogs/no-users-found-dialog/no-users-found-dialog.component';

import { DisableControlDirective } from './directives/disable-control/disable-control.directive';


@NgModule({
  imports: [
    CommonModule,
    MaterialModule,
    RouterModule,
    CountUpModule,
    MomentModule,
    NgxChartsModule,
    LanguageModule,
    ReactiveFormsModule,
    FontAwesomeModule,
    TranslateModule.forChild()
  ],
  exports: [
    ToolbarComponent,
    MaterialModule,
    MomentModule,
    NgxChartsModule,
    FontAwesomeModule,
    RouterModule,
    ReactiveFormsModule,
    FormsModule,
    LanguageModule,
    CardComponent,
    CardTimespanComponent,
    CardNumberComponent,
    MoviePosterComponent,
    PersonPosterComponent,
    ShowPosterComponent,
    LoaderComponent,
    CollectionSelectorComponent,
    NoTypeFoundDialog,
    SyncIsRunningDialog,
    NoUsersFoundDialogComponent,
    CapitalizeFirstPipe,
    ToShorterStringPipe,
    DisableControlDirective,
    SideNavigationComponent,
    CardUserComponent
  ],
  declarations: [
    ToolbarComponent,
    CardComponent,
    CardTimespanComponent,
    CardNumberComponent,
    MoviePosterComponent,
    PersonPosterComponent,
    ShowPosterComponent,
    LoaderComponent,
    CollectionSelectorComponent,
    NoTypeFoundDialog,
    SyncIsRunningDialog,
    CapitalizeFirstPipe,
    ToShorterStringPipe,
    DisableControlDirective,
    UpdateOverlayComponent,
    SideNavigationComponent,
    CardUserComponent,
    NoUsersFoundDialogComponent
  ],
  providers: [
    ToastService,
    EmbyService,
    JobSocketService,
    SystemService,
    UpdateOverlayService,
    SideBarService,
    UpdateService
  ],
  entryComponents: [
    NoTypeFoundDialog,
    SyncIsRunningDialog,
    NoUsersFoundDialogComponent,
    UpdateOverlayComponent]
})
export class SharedModule { }
