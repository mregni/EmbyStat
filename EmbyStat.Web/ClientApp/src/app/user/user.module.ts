import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { SharedModule } from '../shared/shared.module';

import { EmbyService } from '../shared/services/emby.service';
import { UserOverviewComponent } from './user-overview/user-overview.component';
import { UserDetailComponent } from './user-detail/user-detail.component';
import { UserViewsDetailComponent } from './user-views-detail/user-views-detail.component';

@NgModule({
  imports: [
    CommonModule,
    SharedModule,
    TranslateModule
  ],
  providers: [
    EmbyService
  ],
  declarations: [
    UserOverviewComponent,
    UserDetailComponent,
    UserViewsDetailComponent
  ]
})
export class UserModule { }
