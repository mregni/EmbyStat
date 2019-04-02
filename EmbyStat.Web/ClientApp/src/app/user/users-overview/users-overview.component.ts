import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { Router } from '@angular/router';
import { MatDialog } from '@angular/material';
import * as _ from 'lodash';
import { TranslateService } from '@ngx-translate/core';

import { EmbyService } from '../../shared/services/emby.service';
import { EmbyUser } from '../../shared/models/emby/emby-user';
import { NoUsersFoundDialogComponent } from '../../shared/dialogs/no-users-found-dialog/no-users-found-dialog.component'

@Component({
  selector: 'app-users-overview',
  templateUrl: './users-overview.component.html',
  styleUrls: ['./users-overview.component.scss']
})
export class UsersOverviewComponent implements OnInit, OnDestroy {
  private usersSub: Subscription;
  private transSub: Subscription;

  users: EmbyUser[];
  deletedUsers: EmbyUser[];
  defaultValue = "name";

  orderOptions = [];

  constructor(
    private readonly embyService: EmbyService,
    private readonly router: Router,
    private readonly dialog: MatDialog,
    private readonly translateService: TranslateService) {
    this.transSub = this.translateService.get(
      ['USERS.SORTING.USERNAMEASC', 'USERS.SORTING.USERNAMEDESC',
        'USERS.SORTING.LASTACTIVEASC', 'USERS.SORTING.LASTACTIVEDESC']).subscribe((value) => {
          this.orderOptions.push({ key: value['USERS.SORTING.USERNAMEASC'], value: 'name' });
          this.orderOptions.push({ key: value['USERS.SORTING.USERNAMEDESC'], value: 'nameDesc' });
          this.orderOptions.push({ key: value['USERS.SORTING.LASTACTIVEASC'], value: 'lastActivityDate' });
          this.orderOptions.push({ key: value['USERS.SORTING.LASTACTIVEDESC'], value: 'lastActivityDateDesc' });
        });

    this.usersSub = this.embyService.getUsers().subscribe((users: EmbyUser[]) => {
      if (users.length > 0) {
        this.users = _.orderBy(users.filter(x => !x.deleted), ["name"], 'asc');
        this.deletedUsers = _.orderBy(users.filter(x => x.deleted), ["name"], 'asc');
      } else {
        this.dialog.open(NoUsersFoundDialogComponent,
          {
            width: '550px'
          });
      }
    });
  }

  ngOnInit() {
  }

  filterChanged(event: any) {
    var order = 'asc';
    var prop = event.value;
    if (event.value.endsWith('Desc')) {
      order = 'desc';
      prop = event.value.slice(0, -4);
    }
    this.users = _.orderBy(this.users, [prop], order);
    this.deletedUsers = _.orderBy(this.deletedUsers, [prop], order);
  }

  navigateToUser(id: any) {
    this.router.navigate(['user/' + id + '/details']);
  }

  ngOnDestroy() {
    if (this.usersSub !== undefined) {
      this.usersSub.unsubscribe();
    }

    if (this.transSub !== undefined) {
      this.transSub.unsubscribe();
    }
  }
}
