import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { FlexLayoutModule } from '@angular/flex-layout';
import { HttpClientModule, HttpClient } from '@angular/common/http';
import { NgProgressModule } from '@ngx-progressbar/core';
import { NgProgressHttpModule } from '@ngx-progressbar/http';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { TranslateModule, TranslateLoader } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { TooltipModule } from 'ng2-tooltip-directive';
import { library } from '@fortawesome/fontawesome-svg-core';
import { faUserTie, faUserLock, faEyeSlash, faPlay } from '@fortawesome/free-solid-svg-icons';

import { AppComponent } from './app.component';
import { SharedModule } from './shared/shared.module';
import { DashboardModule } from './dashboard/dashboard.module';
import { WizardModule } from './wizard/wizard.module';
import { ServerModule } from './server/server.module';
import { PluginModule } from './plugin/plugin.module';
import { JobsModule } from './jobs/jobs.module';
import { MovieModule } from './movie/movie.module';
import { ShowModule } from './show/show.module';
import { LogsModule } from './logs/logs.module';
import { AboutModule } from './about/about.module';
import { SettingsModule } from './settings/settings.module';
import { UserModule } from './user/user.module';

import { StoreModule } from '@ngrx/store';
import { EffectsModule } from '@ngrx/effects';
import { StoreDevtoolsModule } from '@ngrx/store-devtools';

import { ROOT_REDUCER, META_REDUCERS } from './states/app.state';
import { SettingsEffects } from './settings/state/effects.settings';
import { ServerEffects } from './server/state/effects.server';
import { AboutEffects } from './about/state/effects.about';

import { environment } from '../environments/environment';
import { AppRoutingModule } from './app-routing.module';
import { ErrorInterceptor } from './shared/error.interceptor';

import { SyncGuard } from './shared/guards/sync.guard';

export function createTranslateLoader(http: HttpClient) {
  return new TranslateHttpLoader(http, '/assets/i18n/', '.json');
}

library.add(faUserTie, faUserLock, faEyeSlash, faPlay);

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    FlexLayoutModule,
    HttpClientModule,
    SharedModule,
    SettingsModule,
    DashboardModule,
    WizardModule,
    ServerModule,
    PluginModule,
    JobsModule,
    ShowModule,
    MovieModule,
    UserModule,
    LogsModule,
    TooltipModule,
    AboutModule,
    AppRoutingModule,
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
    EffectsModule.forRoot([SettingsEffects, ServerEffects, AboutEffects]),
    !environment.production ? StoreDevtoolsModule.instrument({ maxAge: 15 }) : []
  ],
  providers: [
    SyncGuard,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: ErrorInterceptor,
      multi: true
    }],
  bootstrap: [AppComponent]
})
export class AppModule { }
