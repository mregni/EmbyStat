import { environment } from 'src/environments/environment';

import { HTTP_INTERCEPTORS, HttpClient } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { FaIconLibrary, FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import {
  faBars, faBirthdayCake, faCogs, faEye, faEyeSlash, faFile, faFilm, faHome, faInfo, faPlayCircle,
  faPuzzlePiece, faServer, faStopwatch, faTv, faUserLock, faUsers, faUserTie
} from '@fortawesome/free-solid-svg-icons';
import { EffectsModule } from '@ngrx/effects';
import { StoreModule } from '@ngrx/store';
import { StoreDevtoolsModule } from '@ngrx/store-devtools';
import { NgProgressModule } from '@ngx-progressbar/core';
import { NgProgressHttpModule } from '@ngx-progressbar/http';
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { AboutModule } from './pages/about/about.module';
import { DashboardModule } from './pages/dashboard/dashboard.module';
import { JobsModule } from './pages/jobs/jobs.module';
import { LogsModule } from './pages/logs/logs.module';
import { MovieModule } from './pages/movie/movie.module';
import { PluginModule } from './pages/plugin/plugin.module';
import { ServerModule } from './pages/server/server.module';
import { EmbyServerInfoEffects } from './pages/server/state/emby-server.effects';
import { SettingsModule } from './pages/settings/settings.module';
import { SettingsEffects } from './pages/settings/state/settings.effects';
import { ShowModule } from './pages/show/show.module';
import { UsersModule } from './pages/users/users.module';
import { WizardModule } from './pages/wizard/wizard.module';
import { OptionsService } from './shared/components/charts/options/options';
import { ErrorInterceptor } from './shared/interceptors/error.interceptor';
import { SharedModule } from './shared/shared.module';
import { META_REDUCERS, ROOT_REDUCER } from './states/app.state';

export function createTranslateLoader(http: HttpClient) {
  return new TranslateHttpLoader(http, '/assets/i18n/', '.json');
}

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    AppRoutingModule,
    SharedModule.forRoot(),
    AboutModule,
    DashboardModule,
    SettingsModule,
    MovieModule,
    PluginModule,
    LogsModule,
    ServerModule,
    ShowModule,
    JobsModule,
    UsersModule,
    WizardModule,
    TranslateModule.forRoot({
      loader: {
        provide: TranslateLoader,
        useFactory: (createTranslateLoader),
        deps: [HttpClient]
      }
    }),
    NgProgressModule,
    NgProgressHttpModule,
    StoreModule.forRoot(ROOT_REDUCER, { metaReducers: META_REDUCERS }),
    EffectsModule.forRoot([SettingsEffects, EmbyServerInfoEffects]),
    !environment.production ? StoreDevtoolsModule.instrument({ maxAge: 15 }) : [],
    FontAwesomeModule
  ],
  schemas: [],
  providers: [
    OptionsService, {
      provide: HTTP_INTERCEPTORS,
      useClass: ErrorInterceptor,
      multi: true
    }
  ],
  bootstrap: [AppComponent]
})
export class AppModule {
  constructor(library: FaIconLibrary) {
    library.addIcons(faHome, faBirthdayCake, faFilm, faPlayCircle, faUserTie, faUserLock,
      faTv, faUsers, faPuzzlePiece, faServer, faCogs, faStopwatch, faFile, faInfo,
      faBars, faEye, faEyeSlash);
  }
}
