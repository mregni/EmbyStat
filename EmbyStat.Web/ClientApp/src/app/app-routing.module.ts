import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { AboutOverviewComponent } from './pages/about/about-overview/about-overview.component';
import {
    DashboardOverviewComponent
} from './pages/dashboard/dashboard-overview/dashboard-overview.component';
import { JobsOverviewComponent } from './pages/jobs/jobs-overview/jobs-overview.component';
import { LogsOverviewComponent } from './pages/logs/logs-overview/logs-overview.component';
import { MovieOverviewComponent } from './pages/movie/movie-overview/movie-overview.component';
import { PluginOverviewComponent } from './pages/plugin/plugin-overview/plugin-overview.component';
import { ServerOverviewComponent } from './pages/server/server-overview/server-overview.component';
import {
    SettingsOverviewComponent
} from './pages/settings/settings-overview/settings-overview.component';
import { ShowOverviewComponent } from './pages/show/show-overview/show-overview.component';
import { UserDetailComponent } from './pages/users/components/user-detail/user-detail.component';
import {
    UserViewsDetailComponent
} from './pages/users/components/user-views-detail/user-views-detail.component';
import { UserContainerComponent } from './pages/users/user-container/user-container.component';
import { UsersOverviewComponent } from './pages/users/users-overview/users-overview.component';
import { WizardOverviewComponent } from './pages/wizard/wizard-overview/wizard-overview.component';
import { SyncGuard } from './shared/guards/sync.guard';

const routes: Routes = [
  { path: '', component: DashboardOverviewComponent },
  { path: 'about', component: AboutOverviewComponent },
  { path: 'logs', component: LogsOverviewComponent },
  { path: 'jobs', component: JobsOverviewComponent },
  { path: 'plugins', component: PluginOverviewComponent },
  { path: 'server', component: ServerOverviewComponent },
  { path: 'movies', component: MovieOverviewComponent, canActivate: [SyncGuard] },
  { path: 'shows', component: ShowOverviewComponent, canActivate: [SyncGuard] },
  { path: 'settings', component: SettingsOverviewComponent },
  { path: 'settings/:tab', component: SettingsOverviewComponent },
  { path: 'users', component: UsersOverviewComponent },
  {
    path: 'user/:id', component: UserContainerComponent, children: [
      { path: 'detail', component: UserDetailComponent },
      { path: 'views', component: UserViewsDetailComponent }
    ]
  },
  { path: 'wizard', component: WizardOverviewComponent },
  { path: '**', redirectTo: '' }];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
