import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import * as _ from 'lodash';

import { EmbyService } from '../../shared/services/emby.service';
import { EmbyUser } from '../../shared/models/emby/emby-user';

@Component({
  selector: 'app-user-overview',
  templateUrl: './user-overview.component.html',
  styleUrls: ['./user-overview.component.scss']
})
export class UserOverviewComponent implements OnInit, OnDestroy {
  private usersSub: Subscription;

  users: EmbyUser[];
  deletedUsers: EmbyUser[];
  defaultValue = "name";

  constructor(
    private readonly embyService: EmbyService) {
    this.usersSub = this.embyService.getUsers().subscribe((users: EmbyUser[]) => {
      this.users = _.orderBy(users.filter(x => !x.deleted), ["name"], 'asc');
      console.log(this.users);
      this.deletedUsers = _.orderBy(users.filter(x => x.deleted), ["name"], 'asc');
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

  ngOnDestroy() {
    if (this.usersSub !== undefined) {
      this.usersSub.unsubscribe();
    }
  }
}
