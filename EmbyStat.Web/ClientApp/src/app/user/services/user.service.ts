import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

import { EmbyUser } from '../../shared/models/emby/emby-user';

@Injectable()
export class UserService {
  user = new BehaviorSubject<EmbyUser>(undefined);

  userChanged(user: EmbyUser) {
    this.user.next(user);
  }
}
