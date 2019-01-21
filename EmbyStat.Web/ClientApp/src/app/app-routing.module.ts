import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { DashboardOverviewComponent } from './dashboard/dashboard-overview/dashboard-overview.component';
import { ConfigurationOverviewComponent } from './configuration/configuration-overview/configuration-overview.component';
import { WizardOverviewComponent } from './wizard/wizard-overview/wizard-overview.component';
import { PluginOverviewComponent } from './plugin/plugin-overview/plugin-overview.component';
import { ServerOverviewComponent } from './server/server-overview/server-overview.component';
import { JobsOverviewComponent } from './jobs/jobs-overview/jobs-overview.component';
import { MovieOverviewComponent } from './movie/movie-overview/movie-overview.component';
import { ShowOverviewComponent } from './show/show-overview/show-overview.component';
import { LogsOverviewComponent } from './logs/logs-overview/logs-overview.component';
import { AboutOverviewComponent } from './about/about-overview/about-overview.component';

import { SyncGuard } from './shared/guards/sync.guard';

const routes: Routes = [{ path: '', component: DashboardOverviewComponent },
  { path: 'configuration', component: ConfigurationOverviewComponent },
  { path: 'configuration/:tab', component: ConfigurationOverviewComponent },
  { path: 'plugin', component: PluginOverviewComponent },
  { path: 'server', component: ServerOverviewComponent },
  { path: 'wizard', component: WizardOverviewComponent },
  { path: 'jobs', component: JobsOverviewComponent },
  { path: 'movie', component: MovieOverviewComponent, canActivate: [SyncGuard] },
  { path: 'movie/:tab', component: MovieOverviewComponent, canActivate: [SyncGuard] },
  { path: 'show', component: ShowOverviewComponent, canActivate: [SyncGuard] },
  { path: 'show/:tab', component: ShowOverviewComponent, canActivate: [SyncGuard] },
  { path: 'logs', component: LogsOverviewComponent },
  { path: 'about', component: AboutOverviewComponent },
  { path: '**', redirectTo: '' }];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
