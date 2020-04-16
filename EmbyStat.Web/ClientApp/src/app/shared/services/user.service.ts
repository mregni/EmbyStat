import { BehaviorSubject } from 'rxjs';

import { Injectable } from '@angular/core';

import { MediaServerUser } from '../models/media-server/media-server-user';

@Injectable()
export class UserService {
  user = new BehaviorSubject<MediaServerUser>(undefined);

  userChanged(user: MediaServerUser): void {
    this.user.next(user);
  }
}
