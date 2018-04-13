import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { DashboardComponent } from './dashboard/dashboard.component';
import { ConfigurationComponent } from './configuration/configuration.component';
import { WizardComponent } from './wizard/wizard.component';
import { PluginComponent } from './plugin/plugin.component';
import { ServerComponent } from './server/server.component';
import { TaskComponent } from './task/task.component';
import { MovieStatComponent } from './movie/movie-stat/movie-stat.component';

const routes: Routes = [{ path: '', component: DashboardComponent },
  { path: 'configuration', component: ConfigurationComponent },
  { path: 'plugin', component: PluginComponent },
  { path: 'server', component: ServerComponent },
  { path: 'wizard', component: WizardComponent },
  { path: 'task', component: TaskComponent },
  { path: 'movie', component: MovieStatComponent },
  { path: '**', redirectTo: '/' }];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
