import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { DashboardOverviewComponent } from './dashboard/dashboard-overview/dashboard-overview.component';
import { SettingsOverviewComponent } from './settings/settings-overview/settings-overview.component';
import { WizardOverviewComponent } from './wizard/wizard-overview/wizard-overview.component';
import { PluginOverviewComponent } from './plugin/plugin-overview/plugin-overview.component';
import { ServerOverviewComponent } from './server/server-overview/server-overview.component';
import { JobsOverviewComponent } from './jobs/jobs-overview/jobs-overview.component';
import { MovieOverviewComponent } from './movie/movie-overview/movie-overview.component';
import { ShowOverviewComponent } from './show/show-overview/show-overview.component';
import { LogsOverviewComponent } from './logs/logs-overview/logs-overview.component';
import { AboutOverviewComponent } from './about/about-overview/about-overview.component';

import { UsersOverviewComponent } from './user/users-overview/users-overview.component';
import { UserDetailComponent } from './user/user-detail/user-detail.component';
import { UserViewsDetailComponent } from './user/user-views-detail/user-views-detail.component';
import { UserContainerComponent } from './user/user-container/user-container.component';

import { SyncGuard } from './shared/guards/sync.guard';

const routes: Routes = [{ path: '', component: DashboardOverviewComponent },
{ path: 'settings', component: SettingsOverviewComponent },
{ path: 'settings/:tab', component: SettingsOverviewComponent },
{ path: 'plugin', component: PluginOverviewComponent },
{ path: 'server', component: ServerOverviewComponent },
{ path: 'wizard', component: WizardOverviewComponent },
{ path: 'jobs', component: JobsOverviewComponent },
{ path: 'movies', component: MovieOverviewComponent, canActivate: [SyncGuard] },
{ path: 'movies/:tab', component: MovieOverviewComponent, canActivate: [SyncGuard] },
{ path: 'shows', component: ShowOverviewComponent, canActivate: [SyncGuard] },
{ path: 'shows/:tab', component: ShowOverviewComponent, canActivate: [SyncGuard] },
{ path: 'logs', component: LogsOverviewComponent },
{ path: 'about', component: AboutOverviewComponent },

{ path: 'users', component: UsersOverviewComponent },
{
  path: 'user/:id', component: UserContainerComponent, children: [
    { path: '', component: UserDetailComponent },
    { path: 'views', component: UserViewsDetailComponent }
  ]
},

{ path: '**', redirectTo: '' }];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
