import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { DashboardComponent } from './dashboard/dashboard.component';
import { ConfigurationOverviewComponent } from './configuration/configuration-overview/configuration-overview.component';
import { WizardOverviewComponent } from './wizard/wizard-overview/wizard-overview.component';
import { PluginComponent } from './plugin/plugin.component';
import { ServerComponent } from './server/server.component';
import { TaskComponent } from './task/task.component';
import { MovieOverviewComponent } from './movie/movie-overview/movie-overview.component';
import { ShowOverviewComponent } from './show/show-overview/show-overview.component';
import { LogsComponent } from './logs/logs.component';
import { AboutOverviewComponent } from './about/about-overview/about-overview.component';

import { SyncGuard } from './shared/guards/sync.guard';

const routes: Routes = [{ path: '', component: DashboardComponent },
  { path: 'configuration', component: ConfigurationOverviewComponent },
  { path: 'plugin', component: PluginComponent },
  { path: 'server', component: ServerComponent },
  { path: 'wizard', component: WizardOverviewComponent },
  { path: 'task', component: TaskComponent },
  { path: 'movie', component: MovieOverviewComponent, canActivate: [SyncGuard] },
  { path: 'show', component: ShowOverviewComponent, canActivate: [SyncGuard] },
  { path: 'logs', component: LogsComponent },
  { path: 'about', component: AboutOverviewComponent },
  { path: '**', redirectTo: '' }];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
