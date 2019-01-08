import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MaterialModule } from './material.module';
import { TranslateModule } from '@ngx-translate/core';
import { CountUpModule } from 'countup.js-angular2';
import { MomentModule } from 'ngx-moment';
import { NgxChartsModule } from '@swimlane/ngx-charts';
import { ChartModule } from 'angular-highcharts';
import { LanguageModule } from './components/language/language.module';
import { ReactiveFormsModule } from '@angular/forms';

import { ToolbarComponent } from './toolbar/toolbar.component';
import { SideNavComponent } from './side-nav/side-nav.component';
import { CardComponent } from './components/card/card.component';
import { CardTimespanComponent } from './components/card-timespan/card-timespan.component';
import { CardNumberComponent } from './components/card-number/card-number.component';
import { MoviePosterComponent } from './components/movie-poster/movie-poster.component';
import { PersonPosterComponent } from './components/person-poster/person-poster.component';
import { ShowPosterComponent } from './components/show-poster/show-poster.component';
import { LoaderComponent } from './components/loader/loader.component';
import { CollectionSelectorComponent } from './components/collection-selector/collection-selector.component';
import { SpinnerOverlayComponent } from './components/spinner-overlay/spinner-overlay.component';

import { CapitalizeFirstPipe } from './pipes/capitalizefirst.pipe';
import { ToShorterStringPipe } from './pipes/shorten-string.pipe';

import { ToastService } from './services/toast.service';
import { TaskSignalService } from './services/signalR/task-signal.service';
import { EmbyService } from './services/emby.service';
import { UpdateService } from './services/update.service';
import { SpinnerOverlayService } from './services/spinner-overlay.service';
import { ToolbarFacade } from './toolbar/state/facade.toolbar';

import { NoTypeFoundDialog } from './dialogs/no-type-found/no-type-found.component';
import { SyncIsRunningDialog } from './dialogs/sync-is-running/sync-is-running.component';

import { DisableControlDirective } from './directives/disable-control/disable-control.directive';

@NgModule({
  imports: [
    CommonModule,
    MaterialModule,
    RouterModule,
    CountUpModule,
    MomentModule,
    NgxChartsModule,
    ChartModule,
    LanguageModule,
    ReactiveFormsModule,
    TranslateModule.forChild()
  ],
  exports: [
    SideNavComponent,
    ToolbarComponent,
    MaterialModule,
    MomentModule,
    NgxChartsModule,
    ChartModule,
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
    CapitalizeFirstPipe,
    ToShorterStringPipe,
    DisableControlDirective,
    SpinnerOverlayComponent
  ],
  declarations: [
    ToolbarComponent,
    SideNavComponent,
    CardComponent,
    CardTimespanComponent,
    CardNumberComponent,
    MoviePosterComponent,
    PersonPosterComponent,
    ShowPosterComponent,
    SpinnerOverlayComponent,
    LoaderComponent,
    CollectionSelectorComponent,
    NoTypeFoundDialog,
    SyncIsRunningDialog,
    CapitalizeFirstPipe,
    ToShorterStringPipe,
    DisableControlDirective
  ],
  providers: [
    ToastService,
    EmbyService,
    TaskSignalService,
    ToolbarFacade,
    UpdateService,
    SpinnerOverlayService
  ],
  entryComponents: [
    NoTypeFoundDialog,
    SyncIsRunningDialog,
    SpinnerOverlayComponent]
})
export class SharedModule { }
