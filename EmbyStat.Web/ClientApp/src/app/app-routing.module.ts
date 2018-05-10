import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { DashboardComponent } from './dashboard/dashboard.component';
import { ConfigurationOverviewComponent } from './configuration/configuration-overview/configuration-overview.component';
import { WizardComponent } from './wizard/wizard.component';
import { PluginComponent } from './plugin/plugin.component';
import { ServerComponent } from './server/server.component';
import { TaskComponent } from './task/task.component';
import { MovieOverviewComponent } from './movie/movie-overview/movie-overview.component';
import { ShowOverviewComponent } from './show/show-overview/show-overview.component';

const routes: Routes = [{ path: '', component: DashboardComponent },
  { path: 'configuration', component: ConfigurationOverviewComponent },
  { path: 'plugin', component: PluginComponent },
  { path: 'server', component: ServerComponent },
  { path: 'wizard', component: WizardComponent },
  { path: 'task', component: TaskComponent },
  { path: 'movie', component: MovieOverviewComponent },
  { path: 'show', component: ShowOverviewComponent },
  { path: '**', redirectTo: '/' }];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
