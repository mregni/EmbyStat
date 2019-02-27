import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { SharedModule } from '../shared/shared.module';

import { EmbyService } from '../shared/services/emby.service';
import { UserService } from './services/user.service';
import { UsersOverviewComponent } from './users-overview/users-overview.component';
import { UserDetailComponent } from './user-detail/user-detail.component';
import { UserViewsDetailComponent } from './user-views-detail/user-views-detail.component';
import { UserContainerComponent } from './user-container/user-container.component';

@NgModule({
  imports: [
    CommonModule,
    SharedModule,
    TranslateModule
  ],
  providers: [
    EmbyService,
    UserService
  ],
  declarations: [
    UsersOverviewComponent,
    UserDetailComponent,
    UserViewsDetailComponent,
    UserContainerComponent
  ]
})
export class UserModule { }
