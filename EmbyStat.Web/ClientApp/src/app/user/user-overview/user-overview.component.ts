import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';

import { EmbyService } from '../../shared/services/emby.service';
import { EmbyUser } from '../../shared/models/emby/emby-user';

@Component({
  selector: 'app-user-overview',
  templateUrl: './user-overview.component.html',
  styleUrls: ['./user-overview.component.scss']
})
export class UserOverviewComponent implements OnInit {
  users$: Observable<EmbyUser[]>;

  constructor(
    private readonly embyService: EmbyService) {
    this.users$ = this.embyService.getUsers();
  }

  ngOnInit() {
  }
}
