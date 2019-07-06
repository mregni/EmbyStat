import { BehaviorSubject } from 'rxjs';

import { Injectable } from '@angular/core';

import { EmbyUser } from '../models/emby/emby-user';

@Injectable()
export class UserService {
  user = new BehaviorSubject<EmbyUser>(undefined);

  userChanged(user: EmbyUser) {
    this.user.next(user);
  }
}
