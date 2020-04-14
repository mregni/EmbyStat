import { SharedModule } from 'src/app/shared/shared.module';

import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';

import { UserDetailComponent } from './components/user-detail/user-detail.component';
import {
  UserViewsDetailComponent
} from './components/user-views-detail/user-views-detail.component';
import { UserContainerComponent } from './user-container/user-container.component';
import { UsersOverviewComponent } from './users-overview/users-overview.component';

@NgModule({
  declarations: [
    UsersOverviewComponent,
    UserDetailComponent,
    UserViewsDetailComponent,
    UserContainerComponent
  ],
  imports: [
    CommonModule,
    TranslateModule,
    SharedModule
  ]
})
export class UsersModule { }
