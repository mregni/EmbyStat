import { Routes } from '@angular/router';
import { DashboardComponent } from './dashboard/dashboard.component';
import { ConfigurationComponent } from './configuration/configuration.component';

export const APP_ROUTES: Routes = [
  { path: '', component: DashboardComponent },
  { path: 'configuration', component: ConfigurationComponent },
  { path: '**', redirectTo: '/' }
];
