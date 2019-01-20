import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs/BehaviorSubject';

@Injectable()
export class SideBarService {
  menuVisibleSubject = new BehaviorSubject<boolean>(true);

  closeMenu() {
    this.menuVisibleSubject.next(false);
  }

  openMenu() {
    this.menuVisibleSubject.next(true);
  }
}
