import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { FlexLayoutModule } from '@angular/flex-layout';
import { RouterModule } from '@angular/router';
import { HttpClientModule } from '@angular/common/http';

import { APP_ROUTES } from './app.routes';

import { AppComponent } from './app.component';
import { SharedModule } from './shared/shared.module';
import { ConfigurationModule } from './configuration/configuration.module';
import { DashboardModule } from './dashboard/dashboard.module';

import { StoreModule } from '@ngrx/store';
import { EffectsModule } from '@ngrx/effects';
import { StoreDevtoolsModule } from '@ngrx/store-devtools';

import { ROOT_REDUCER, META_REDUCERS } from './states/app.state';
import { ConfigurationFacade } from './states/configuration/configuration.facade';

import { environment } from '../environments/environment';


@
NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    FlexLayoutModule,
    HttpClientModule,
    SharedModule,
    ConfigurationModule,
    DashboardModule,
    RouterModule.forRoot(APP_ROUTES),
    StoreModule.forRoot(ROOT_REDUCER, { metaReducers: META_REDUCERS }),
    EffectsModule.forRoot([ConfigurationFacade]),
    !environment.production ? StoreDevtoolsModule.instrument({ maxAge: 5 }) : []
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
