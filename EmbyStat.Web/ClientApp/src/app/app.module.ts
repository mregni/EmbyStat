import { HttpClient } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { AboutModule } from './pages/about/about.module';
import { DashboardModule } from './pages/dashboard/dashboard.module';
import { LogsModule } from './pages/logs/logs.module';
import { MovieModule } from './pages/movie/movie.module';
import { PluginModule } from './pages/plugin/plugin.module';
import { ServerModule } from './pages/server/server.module';
import { SettingsModule } from './pages/settings/settings.module';
import { MenuItems } from './shared/injectables/menu-items';
import { SharedModule } from './shared/shared.module';

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
    TranslateModule.forRoot({
      loader: {
        provide: TranslateLoader,
        useFactory: (createTranslateLoader),
        deps: [HttpClient]
      }
    })
  ],
  schemas: [],
  providers: [
    MenuItems],
  bootstrap: [AppComponent]
})
export class AppModule { }
